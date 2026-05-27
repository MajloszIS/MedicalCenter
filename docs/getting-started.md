Instrukcja uruchomienia projektu MedicalCenter lokalnie.

## Wymagania

- .NET 9 SDK
- Docker Desktop (dla bazy SQL Server)
- Visual Studio 2022 lub VS Code z rozszerzeniem C# Dev Kit
- Git

## Uruchomienie lokalne

### 1. Sklonuj repozytorium

```bash
git clone https://github.com/MajloszIS/MedicalCenter.git
cd MedicalCenter
```

### 2. Uruchom kontener z bazą danych

W głównym katalogu repo (tam, gdzie jest `docker-compose.yml`):

```bash
docker-compose up -d
```

Kontener z SQL Server uruchomi się w tle na porcie `1433`.

### 3. Wykonaj migracje bazy danych

```bash
cd MedicalCenter
dotnet ef database update
```

Migracje utworzą tabele oraz wstawią dane testowe (konta użytkowników, lekarzy, leki, specjalizacje).

### 4. Skonfiguruj sekrety aplikacji

Aplikacja używa `user-secrets` do przechowywania kluczy. Ustaw następujące wartości w katalogu projektu:

```bash
dotnet user-secrets set "Stripe:SecretKey" "sk_test_..."
dotnet user-secrets set "Stripe:PublishableKey" "pk_test_..."
dotnet user-secrets set "Authentication:Google:ClientId" "..."
dotnet user-secrets set "Authentication:Google:ClientSecret" "..."
dotnet user-secrets set "Jwt:Key" "..."
```

### 5. Uruchom aplikację

```bash
dotnet run
```

Aplikacja będzie dostępna pod adresem `https://localhost:5029`.

## Konta testowe

Wszystkie konta używają tego samego hasła testowego.

| Rola    | Email                | Opis                                    |
|---------|----------------------|-----------------------------------------|
| Admin   | admin@medical.pl     | Pełny dostęp do panelu administratora   |
| Lekarz  | lekarz@medical.pl    | Zarządzanie wizytami, diagnozy, recepty |
| Pacjent | pacjent@medical.pl   | Umawianie wizyt, koszyk, zamówienia     |
| Kurier  | kurier@medical.pl    | Realizacja dostaw                       |

## Dokumentacja API

Po uruchomieniu aplikacji dokumentacja Swagger dostępna jest pod adresem:

```
https://localhost:5029/swagger
```
