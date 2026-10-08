# Pedros Cantina

C# konsolprogram til vagtplanlægning for caféen Pedros Cantina.
Projektet bruger SQL Server og ADO.NET (`Microsoft.Data.SqlClient`).

## Krav

- Visual Studio 2022 med workload **.NET desktop development**
- .NET 8
- SQL Server (LocalDB, Express eller fuld instans)
- SQL Server Management Studio (SSMS) eller en anden SQL-klient

## Opret databasen

Kør SQL-scripts i denne rækkefølge i SSMS:

1. `PedrosCantina/Sql/CreateDatabase.sql`
2. `PedrosCantina/Sql/CreateTables.sql`
3. `PedrosCantina/Sql/InsertTestData.sql`

`Queries.sql` er dokumentation af de forespørgsler, programmet bruger. Den skal ikke køres som ét samlet script, fordi den indeholder `@`-parametre.

## Connection string

Connection string ligger i:

`PedrosCantina/Database.cs`

Standard:

```
Server=(localdb)\MSSQLLocalDB;Database=PedrosCantina;Trusted_Connection=True;TrustServerCertificate=True;
```

Ret `Server=(localdb)\MSSQLLocalDB`, hvis din SQL Server hedder noget andet, for eksempel:

- `Server=localhost;`
- `Server=.\SQLEXPRESS;`
- `Server=NAVNET-PÅ-DIN-PC;`

Databasenavnet skal matche det, der oprettes i `CreateDatabase.sql` (`PedrosCantina`).

Programmet bruger Windows Authentication (`Trusted_Connection=True`). Der ligger ingen adgangskode i projektet.

## Start programmet i Visual Studio

1. Åbn `PedrosCantina.sln`.
2. Sørg for at `PedrosCantina` er startprojekt.
3. Tryk F5 eller klik **Start**.

Første gang henter Visual Studio NuGet-pakken `Microsoft.Data.SqlClient` automatisk.

## Testdata

`InsertTestData.sql` opretter:

- Pedro og Jacoby som ledere
- Maria, Anders, Sofia og Emil som almindelige medarbejdere
- Udvalgte dage i januar, marts, juni, oktober og december 2026
- Bemanding med 2 personer på hverdage og 3 i weekenden, altid med mindst én leder

Brug fx år **2026** og måned **10**, når du tester månedsplan og belastning.
