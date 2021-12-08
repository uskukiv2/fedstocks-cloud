using System;
using System.Collections;
using MediatR;

namespace fed.cloud.product.application.Models;

public class CountryDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public CountyDto[] CountyNumbers { get; set; }
}

public class CountyDto
{
    public int NumberInCountry { get; set; }

    public string Name { get; set; }
}