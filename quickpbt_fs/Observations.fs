namespace quickpbt

open FsCheck
open FsCheck.Xunit

open DomainUnderTest

/// contains examples of gathering diagnostics about generated data
/// (NOTE: `dotnet test` requires a `--verbosity` of *at least* 'normal' to see observations on the command line)
[<Properties(Arbitrary=[| typeof<Generator> |])>]
module Observations =
  /// a trival observation partitions data into one of two buckets
  [<Property>]
  let ``trivial daylight savings support`` (anyDate :Date) (anyZone :Zone) (NonNegativeInt total) =
    let days = Time.FromDays(total)

    let addThenShift = Zone.ConvertTime(anyDate + days, anyZone)
    let shiftThenAdd = Zone.ConvertTime(anyDate, anyZone) + days

    addThenShift = shiftThenAdd
      |> Prop.trivial anyZone.SupportsDaylightSavingTime

  /// a classification partitions data into one of N, labelled buckets
  [<Property>]
  let ``classify meridian position`` (anyDate :Date) (anyZone :Zone) (NonNegativeInt total) =
    let days = Time.FromDays(total)

    let addThenShift = Zone.ConvertTime(anyDate + days, anyZone)
    let shiftThenAdd = Zone.ConvertTime(anyDate, anyZone) + days

    addThenShift = shiftThenAdd
      |> Prop.classify (anyDate.Offset < Time.Zero) "West of Greenwich"
      |> Prop.classify (anyDate.Offset = Time.Zero) "Within Greenwich"
      |> Prop.classify (anyDate.Offset > Time.Zero) "East of Greenwich"

  /// rather than using a boolean observation, collect reports any value
  [<Property>]
  let ``collect weekday name`` (anyDate :Date) (anyZone :Zone) (NonNegativeInt total) =
    let days = Time.FromDays(total)

    let addThenShift = Zone.ConvertTime(anyDate + days, anyZone)
    let shiftThenAdd = Zone.ConvertTime(anyDate, anyZone) + days

    addThenShift = shiftThenAdd
      |> Prop.collect (weekdayName anyDate)

  /// observations may be combined as much as is desired
  [<Property>]
  let ``many observations combined`` (anyDate :Date) (anyZone :Zone) (NonNegativeInt total) =
    let days = Time.FromDays(total)

    let addThenShift = Zone.ConvertTime(anyDate + days, anyZone)
    let shiftThenAdd = Zone.ConvertTime(anyDate, anyZone) + days

    addThenShift = shiftThenAdd
      |> Prop.trivial   anyZone.SupportsDaylightSavingTime
      |> Prop.classify  (anyDate.Offset < Time.Zero) "West of Greenwich"
      |> Prop.classify  (anyDate.Offset = Time.Zero) "Within Greenwich"
      |> Prop.classify  (anyDate.Offset > Time.Zero) "East of Greenwich"
      |> Prop.collect   (weekdayName anyDate)
