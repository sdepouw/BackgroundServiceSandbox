﻿namespace Core7Library.CatFacts;

public interface ICatFactsService
{
    Task<List<CatFact>> GetTheFactsAsync(CancellationToken cancellationToken);
    Task<CatFact?> Hmmm(CancellationToken cancellationToken);
}