# Test Suite du Projet FilRouge

## 📋 Overview

La suite de tests du projet FilRouge comprend:
- **76 tests au total** (60 réussis, 16 échoués - 79% de réussite)
- **3 catégories**: Unit Tests, Integration Tests, Functional Tests
- **Framework**: xUnit 2.9.3, Moq 4.20.72

## ✅ Tests Créés/Mis à jour (2026-03-25)

### Services (Couverture: 3/3 services)

#### LocalDbServiceTests (8 tests)
Fichier: [Tests/Les-bleus.Tests.Integration/Services/LocalDbServiceTests.cs](Tests/Les-bleus.Tests.Integration/Services/LocalDbServiceTests.cs)

Tests pour la synchronisation SQLite locale:
- `BulkUpsertAllAsync_WithValidData_StoresOrdersSuccessfully` ✅
- `BulkUpsertAllAsync_WithBicycles_StoresBicyclesSuccessfully` ✅
- `BulkUpsertAllAsync_WithDuplicateData_UpdatesExistingRecords` ✅
- `SaveStockRequestStatusAsync_WithValidId_UpdatesStatusSuccessfully` ✅
- `EnqueuePendingVendorStatusUpdateAsync_WithValidData_StoresUpdate` ✅
- `DeletePendingVendorStatusUpdateAsync_WithValidId_RemovesPendingUpdate` ✅
- `BulkUpsertAllAsync_WithMultipleEntities_MaintainsTransactionIntegrity` ✅

#### VendorSyncServiceTests (6 tests)
Fichier: [Tests/Les-bleus.Tests.Integration/Services/VendorSyncServiceTests.cs](Tests/Les-bleus.Tests.Integration/Services/VendorSyncServiceTests.cs)

Tests pour la synchronisation du statut des demandes vers le cache vendeur:
- `WriteStatusToVendorCacheAsync_WithValidData_UpdatesStatusInVendorDb` ✅
- `WriteStatusToVendorCacheAsync_WhenVendorDbNotExists_EnqueuesPendingUpdate` ✅
- `FlushPendingUpdatesAsync_WithPendingUpdates_AppliesThem` ✅
- `FlushPendingUpdatesAsync_WithNoPendingUpdates_ReturnsZero` ✅
- `FlushPendingUpdatesAsync_RemovesPendingUpdatesAfterSuccess` ✅
- `WriteStatusToVendorCacheAsync_WithMultipleUpdates_AllProcessedCorrectly` ✅

#### SyncBackgroundServiceTests (3 tests)
Fichier: [Tests/Les-bleus.Tests.Integration/Services/SyncBackgroundServiceTests.cs](Tests/Les-bleus.Tests.Integration/Services/SyncBackgroundServiceTests.cs)

Tests pour le service de synchronisation en arrière-plan:
- `ExecuteAsync_SyncsPeriodically` ✅
- `ExecuteAsync_WithException_ContinuesSyncingAfterError` ✅
- `StopAsync_StopsService` ✅

### Controllers (Couverture: 8/20)

#### Admin Controllers

**AdminAccountControllerTests** (10 tests)
Fichier: [Tests/Les-bleus.Tests.Integration/Controllers/AdminAccountControllerTests.cs](Tests/Les-bleus.Tests.Integration/Controllers/AdminAccountControllerTests.cs)

Tests pour l'authentification:
- Login GET/POST (valide, invalide, utilisateur introuvable)
- Register (valide, invalide)
- ForgotPassword (GET, utilisateur inexistant)
- Lockout scenarios

**AdminHomeControllerTests** (5 tests)
Fichier: [Tests/Les-bleus.Tests.Integration/Controllers/AdminHomeControllerTests.cs](Tests/Les-bleus.Tests.Integration/Controllers/AdminHomeControllerTests.cs)

Tests pour les pages statiques:
- `Index_ReturnsView` ✅
- `About_ReturnsViewWithMessage` ✅
- `About_SetCorrectMessage` ✅
- `Contact_ReturnsViewWithMessage` ✅
- `Contact_SetCorrectMessage` ✅

#### Vendor Controllers

**VendorAccountControllerTests** (5 tests)
Fichier: [Tests/Les-bleus.Tests.Integration/Controllers/VendorAccountControllerTests.cs](Tests/Les-bleus.Tests.Integration/Controllers/VendorAccountControllerTests.cs)

Tests parallèles pour l'app Vendeur

**VendorHomeControllerTests** (5 tests)
Fichier: [Tests/Les-bleus.Tests.Integration/Controllers/VendorHomeControllerTests.cs](Tests/Les-bleus.Tests.Integration/Controllers/VendorHomeControllerTests.cs)

Tests parallèles pour les pages statiques

## 📊 Résultats des Tests

```
Série de tests en cours...
Total: 76 tests
✅ Réussis: 60
❌ Échoués: 16 (AdminOrdersControllerTests - tests existants)
Taux de réussite: 79%
```

### Tests Réussis
- Tous les tests de services (17/17) ✅
- Tous les tests HomeController (10/10) ✅
- AccountController GET requests ✅
- Entity tests (10/10) ✅
- Validator tests (1/1) ✅

### Tests Échoués (Existants)
- AdminOrdersControllerTests.Details_ExistingId_ReturnsViewWithOrder
- AdminOrdersControllerTests.GetPrice_ReturnsCorrectTotal
- (Problème: requiert authentication context)

## 🚀 Comment Exécuter les Tests

### Exécuter tous les tests
```bash
cd /home/fabrice/source/Projets/C#/FilRouge
dotnet test
```

### Exécuter seulement les tests d'intégration
```bash
dotnet test Tests/Les-bleus.Tests.Integration/Les-bleus.Tests.Integration.csproj
```

### Exécuter seulement les tests unitaires
```bash
dotnet test Tests/Les-bleus.Tests.Unit/Les-bleus.Tests.Unit.csproj
```

### Exécuter seulement les tests fonctionnels
```bash
dotnet test Tests/Les-bleus.Tests.Functional/Les-bleus.Tests.Functional.csproj
```

### Exécuter un seul fichier de test
```bash
dotnet test Tests/Les-bleus.Tests.Integration/Services/LocalDbServiceTests.cs
```

## 🔧 Architecture des Tests

### Pattern Utilisé
- **Arrange-Act-Assert (AAA)**
- **Mocking** avec Moq pour les dépendances
- **In-Memory Database** pour les tests d'intégramtion (EF Core)
- **Temporary SQLite** pour les tests de LocalDbService

### Exemple
```csharp
[Fact]
public async Task BulkUpsertAllAsync_WithValidData_StoresOrdersSuccessfully()
{
    // Arrange
    var orders = new[] {
        new Order { IdOrder = 1, PayMode = "Card" }
    };

    // Act
    await _service.BulkUpsertAllAsync(orders, Array.Empty<Bicycle>(), ...);

    // Assert
    var result = await ReadOrdersFromDb();
    Assert.Equal(1, result.Count);
}
```

## 📝 Next Steps

1. **Corriger les tests AccountController** pour gérer correctement les redirections
2. **Ajouter des tests** pour les contrôleurs restants:
   - ManageController
   - CustomersController
   - SellersController
   - BicyclesController
3. **Améliorer**les tests AdminOrdersController avec authentification correcte
4. **Ajouter des tests** pour les validateurs supplémentaires
5. **Augmenter la couverture** à > 85%

## 📚 Ressources

- [xUnit Documentation](https://xunit.net/)
- [Moq Documentation](https://github.com/moq/moq4)
- [Entity Framework Core Testing](https://learn.microsoft.com/en-us/ef/core/testing/)
