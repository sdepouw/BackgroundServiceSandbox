﻿using System.ComponentModel.DataAnnotations;
using Core7Library.CatFacts;

namespace WorkServiceSeven;

public class MySettings
{
    [Required]
    public string Foo { get; set; } = "";
    public int Bar { get; set; }
    public bool Fizz { get; set; }
    public DateTimeOffset Buzz { get; set; }
    [Required]
    public CatFactsClientSettings CatFactsClientSettings { get; set; } = new();
}