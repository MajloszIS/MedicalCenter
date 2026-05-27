# MedicalCenter

<p align="center">
  <img src="./images/MedicalCenterLogo.png" alt="Logo" width="200" />
</p>

Aplikacja webowa centrum medycznego zbudowana w ASP.NET Core MVC z modułem sprzedaży leków i obsługą dostaw.


## Technologie

| Pakiet | Wersja | Zastosowanie |
|--------|--------|--------------|
| .NET / ASP.NET Core MVC | 9.0 | Framework webowy |
| Entity Framework Core | 9.0 | ORM |
| MS SQL Server (Azure SQL Edge) | Docker | Baza danych |
| BCrypt.Net-Next | 4.1.0 | Hashowanie haseł |
| Microsoft.AspNetCore.Authentication.Google | 9.0.0 | Logowanie przez Google OAuth |
| Microsoft.AspNetCore.Authentication.JwtBearer | 9.0.0 | Autoryzacja JWT dla API |
| Stripe.net | 51.1.0 | Płatności online |
| QuestPDF | 2026.5.0 | Generowanie PDF |
| QRCoder | 1.8.0 | Kody QR na receptach |
| Swashbuckle.AspNetCore | 6.9.0 | Swagger UI |
| Bootstrap | 5 | Stylowanie UI |
| Font Awesome | 6.4.0 | Ikony |
| Flatpickr | — | Kalendarz dat wizyt |

## Architektura

Klasyczna warstwowa architektura MVC:

```
Controllers → Services → Repositories → Database
```

```
Controllers/
  Api/          → endpointy REST API (JWT)
  *.cs          → kontrolery MVC (cookie auth)
Services/       → logika biznesowa + mapowanie DTO
Repositories/   → dostęp do bazy przez EF Core
Models/         → encje bazy danych
DTOs/           → obiekty transferowe między warstwami
Views/          → widoki Razor (.cshtml)
Data/           → AppDbContext
Migrations/     → migracje EF Core
```

Wzorce: **Repository**, **Service**, **Dependency Injection**, **DTO**.

## Encje (modele)

`User`, `Doctor`, `Patient`, `Courier`, `Appointment`, `MedicalRecord`, `Diagnosis`, `Treatment`, `Prescription`, `PrescriptionItem`, `Medicine`, `MedicineCategory`, `Cart`, `CartItem`, `Order`, `OrderItem`, `OrderRating`, `Delivery`, `Address`, `Review`, `MedicalLeave`, `Referral`, `Specializations`, `Department`, `DoctorDepartment`, `Invoice`, `AppointmentStatus`, `OrderStatus`, `Deliverytatus`, `Role`

![Diagram](/docs/RelacyjnyDiagramTabelwBazieDanych.png)

## Funkcjonalności

### Uwierzytelnianie i autoryzacja
- Rejestracja konta pacjenta
- Logowanie i wylogowanie (cookie-based)
- Logowanie przez **Google OAuth**
- Autoryzacja oparta na rolach: `Admin`, `Doctor`, `Patient`, `Courier`
- Hashowanie haseł przez BCrypt
- JWT dla REST API

### Panel pacjenta
- Przeglądanie listy lekarzy ze specjalizacjami i ocenami
- Umawianie wizyt (wybór daty, godziny, opis — kalendarz Flatpickr)
- Przeglądanie i anulowanie własnych wizyt
- Karta medyczna — historia diagnoz, leczenia, recept
- Recepty — lista i pobieranie PDF z kodem QR
- Skierowania — lista i pobieranie PDF
- Zwolnienia lekarskie — lista i pobieranie PDF
- Wystawianie opinii o lekarzach
- Historia zamówień i ocenianie dostawy
- Pobieranie faktury PDF za zamówienie
- Profil — edycja danych, zmiana hasła, zdjęcie profilowe

### Panel lekarza
- Własne wizyty i zarządzanie nimi
- Lista swoich pacjentów
- Karta medyczna pacjenta — dodawanie diagnoz i planów leczenia
- Wystawianie recept z lekami
- Wystawianie skierowań do innych specjalistów
- Wystawianie zwolnień lekarskich
- Profil — edycja danych, zmiana hasła, zdjęcie profilowe

### Panel kuriera
- Lista dostępnych dostaw do przyjęcia
- Lista własnych aktywnych dostaw
- Zmiana statusu dostawy
- Profil — edycja danych

### Panel admina
- Pełny CRUD lekarzy (tworzenie, edycja, usuwanie, przeglądanie wizyt)
- Pełny CRUD pacjentów (edycja, usuwanie, przeglądanie wizyt)
- Pełny CRUD kurierów (tworzenie, edycja, usuwanie, przeglądanie dostaw)
- Pełny CRUD leków (tworzenie, edycja, usuwanie)
- Pełny CRUD specjalizacji Lekarzy (tworzenie, edycja, usuwanie)
- Pełny CRUD departamentów (tworzenie, edycja, usuwanie)
- Pełny CRUD kategorii leków (tworzenie, edycja, usuwanie)
- Przeglądanie wszystkich zamówień
- Zarządzanie opiniami lekarzy (przeglądanie, usuwanie)

### Sklep i płatności
- Przeglądanie leków w aptece z kategoriami
- Koszyk zakupów
- Płatności online przez **Stripe** (tryb testowy)
- Automatyczne generowanie faktury PDF po opłaceniu zamówienia

### REST API (JWT Bearer)

| Endpoint | Metoda | Opis | Rola |
|----------|--------|------|------|
| `POST /api/auth/login` | POST | Logowanie, zwraca JWT | Publiczny |
| `GET /api/doctors` | GET | Lista wszystkich lekarzy | Zalogowany |
| `GET /api/doctors/specializations/{specializationName}` | GET | Lekarze wg specjalizacji | Zalogowany |
| `GET /api/medicines` | GET | Lista leków | Zalogowany |
| `GET /api/medicines/{id}` | GET | Lek po ID | Zalogowany |
| `POST /api/medicines` | POST | Dodaj lek | Admin |
| `PUT /api/medicines/{id}` | PUT | Edytuj lek | Admin |
| `DELETE /api/medicines/{id}` | DELETE | Usuń lek | Admin |
| `GET /api/deliveries/unassigned` | GET | Nieprzypisane dostawy | Admin/Courier |
| `POST /api/deliveries/{id}/accept` | POST | Przyjmij dostawę | Admin/Courier |
| `PATCH /api/deliveries/{id}/status` | PATCH | Zmień status dostawy | Admin/Courier |

Dokumentacja interaktywna: `/swagger`

### Generowanie dokumentów PDF
- **Recepta** — z kodem QR, generowana przez QuestPDF
- **Skierowanie** — pobieralne przez pacjenta
- **Zwolnienie lekarskie** — pobieralne przez pacjenta
- **Faktura** — pobieralne przez pacjenta po opłaceniu zamówienia

## Demo
Widok Pacjenta
![Widok Pacjenta](./images/PatientDemo.gif)

**Demo:** [medicalcenter-app...azurewebsites.net](https://medicalcenter-app-hwb2dqgfcpfub3b5.germanywestcentral-01.azurewebsites.net/)

**Repozytorium:** https://github.com/MajloszIS/MedicalCenter

---

## Instalacja i uruchomienie

### Wymagania

- .NET 9.0 SDK
- Docker Desktop
- Git
- IDE: Visual Studio 2022 / Rider / VS Code

### Pierwsze uruchomienie

**1. Sklonuj repozytorium:**
```bash
git clone https://github.com/MajloszIS/MedicalCenter.git
cd MedicalCenter
```

**2. Uruchom bazę danych w Dockerze:**
```bash
docker compose up -d
```

Kontener: `medicalcenter-db`, port: `1433`.

**3. Zastosuj migracje EF Core (tworzy tabele i seed data):**
```bash
dotnet ef database update --project MedicalCenter
```

**4. Skonfiguruj user secrets:**
```bash
cd MedicalCenter

# Stripe (własne konto testowe — patrz sekcja niżej)
dotnet user-secrets set "Stripe:SecretKey" "sk_test_..."
dotnet user-secrets set "Stripe:PublishableKey" "pk_test_..."

# Google OAuth
dotnet user-secrets set "Authentication:Google:ClientId" "..."
dotnet user-secrets set "Authentication:Google:ClientSecret" "..."

# JWT
dotnet user-secrets set "Jwt:Key" "twój-tajny-klucz-min-32-znaki"
```

**5. Uruchom aplikację:**
```bash
dotnet run --project MedicalCenter
```

**6.** Otwórz przeglądarkę pod adresem `https://localhost:<port>`

---

### Codzienna praca

```bash
# Start bazy
docker compose up -d

# Stop bazy (dane zostają)
docker compose down

# Pełny reset bazy od zera
docker compose down -v && docker compose up -d && dotnet ef database update --project MedicalCenter
```

---

## Konfiguracja sekretów

Sekrety **nie są commitowane** do repozytorium. Każdy developer ustawia je lokalnie przez `dotnet user-secrets`. `UserSecretsId` projektu: `1ba36229-4108-4c94-b3e2-6ad97553c7ad`.

### Stripe (płatności)

1. Załóż darmowe konto na https://stripe.com
2. Włącz **Test mode** → https://dashboard.stripe.com/test/apikeys
3. Skopiuj `Secret key` (`sk_test_...`) i `Publishable key` (`pk_test_...`)
4. Ustaw przez `dotnet user-secrets set` jak wyżej

Testowa karta kredytowa: `4242 4242 4242 4242`, dowolna przyszła data, dowolny CVC.

### Google OAuth

1. Wejdź na https://console.cloud.google.com
2. Utwórz projekt
3. Typ: **Web application**
4. Authorized redirect URI: `https://localhost:<port>/signin-google`
5. Skopiuj **Client ID** i **Client Secret**

### JWT

Klucz powinien mieć minimum 32 znaki. Przykład generowania:
```bash
# PowerShell
[System.Convert]::ToBase64String([System.Security.Cryptography.RandomNumberGenerator]::GetBytes(32))
```

---

## Domyślne konta (seed data)

| Email | Hasło | Rola |
|-------|-------|------|
| admin@medical.pl | admin123 | Admin |
| lekarz@medical.pl | admin123 | Doctor |
| kurier@medical.pl | admin123 | Courier |
| pacjent@medical.pl | admin123 | Patient |

---

## Dokumentacja API

Po uruchomieniu aplikacji, Swagger UI dostępny jest pod:
```
https://localhost:<port>/swagger
```

Aby przetestować chronione endpointy:
1. Wywołaj `POST /api/auth/login` z danymi konta
2. Skopiuj zwrócony token JWT
3. Kliknij przycisk **Authorize** w Swagger UI i wklej token w formacie: `Bearer <token>`
