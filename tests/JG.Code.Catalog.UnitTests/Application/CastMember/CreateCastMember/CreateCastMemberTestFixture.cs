﻿using JG.Code.Catalog.Domain.Enum;
using JG.Code.Catalog.UnitTests.Application.CastMember.Common;

namespace JG.Code.Catalog.UnitTests.Application.CastMember.CreateCastMember;

[CollectionDefinition(nameof(CreateCastMemberTestFixture))]
public class CreateCastMemberTestFixtureCollection : ICollectionFixture<CreateCastMemberTestFixture>
{
}

public class CreateCastMemberTestFixture : CastMemberUseCasesBaseFixture
{
    public string GetValidName() => Faker.Name.FullName();
    public CastMemberType GetRandomCastMemberType() => (CastMemberType)(new Random()).Next(1,2);
}