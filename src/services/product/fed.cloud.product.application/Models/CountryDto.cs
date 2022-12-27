using System;

namespace fed.cloud.product.application.Models;

public class CountryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Number { get; set; }

    public CountyDto[] CountyDtos { get; set; }
}

public class CountyDto
{
    public Guid Id { get; set; }
    public int Number { get; set; }
    public string Name { get; set; }
}