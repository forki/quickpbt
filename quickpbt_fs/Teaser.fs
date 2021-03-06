﻿namespace quickpbt

open Xunit
open FsCheck
open FsCheck.Xunit

open DomainUnderTest

/// contrasts a unit test with a property test
module Teaser =
  let daysInAWeek   =  7
  let hoursInAWeek  = 24 * daysInAWeek

  [<Fact>]
  let ``days should equal hours`` () =
    let today = Date.Now //NOTE: single, hard-coded value

    let days  = today + Time.FromDays(daysInAWeek)
    let hours = today + Time.FromHours(hoursInAWeek)

    Assert.Equal(days, hours)

  [<Property>]
  let ``unit of time should not effect addition`` (anyDate :Date) =
    //NOTE: lots of different, random values
    let days  = anyDate + Time.FromDays(daysInAWeek)
    let hours = anyDate + Time.FromHours(hoursInAWeek)

    days = hours
