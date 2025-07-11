﻿namespace JG.Code.Catalog.Application.Interfaces;

public interface IStorageService
{
    Task<string> Upload(string fileName, Stream stream, CancellationToken cancellationToken);
}