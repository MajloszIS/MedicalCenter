# MedicalCenter
 
Aplikacja webowa centrum medycznego zbudowana w ASP.NET Core MVC z wykorzystaniem Entity Framework Core.
 
## Technologie
 
- **.NET 9.0**
- **ASP.NET Core MVC**
- **Entity Framework Core 9.0** — ORM do obsługi bazy danych
- **SQL Server** — baza danych (LocalDB na Windows, Docker na macOS)
- **BCrypt.Net-Next 4.1.0** — hashowanie haseł
- **Swashbuckle.AspNetCore 10.1.7** — dokumentacja API (Swagger)
- **Bootstrap 5** — framework CSS
## Architektura
 
Aplikacja wykorzystuje architekturę warstwową:
 
```
Controllers → Services → Repositories → Database
```
 
- **Controllers** — obsługa żądań HTTP, zwracanie widoków i odpowiedzi API
- **Services** — logika biznesowa
- **Repositories** — dostęp do bazy danych
- **DTOs** — obiekty transferu danych
## Funkcjonalności
 
### Uwierzytelnianie i autoryzacja
- Rejestracja nowego konta pacjenta
- Logowanie i wylogowanie
- Autoryzacja oparta na rolach (Admin, Doctor, Patient, Courier)
- Sesja oparta na ciasteczkach (Cookie Authentication)
### Panel pacjenta
- Przeglądanie listy lekarzy ze specjalizacjami
- Umawianie wizyt u lekarzy
- Przeglądanie własnych wizyt
- Anulowanie wizyt
### Panel lekarza
- Przeglądanie własnych wizyt
- Przeglądanie listy swoich pacjentów
### Panel admina
- Dodawanie i usuwanie lekarzy
- Przeglądanie listy pacjentów
### API
- Endpointy REST API udokumentowane przez Swagger UI (`/swagger`)
- Pobieranie listy lekarzy
- Pobieranie lekarza po ID
## Wymagania
 
- .NET 9.0 SDK
- SQL Server (LocalDB na Windows lub Docker na macOS/Linux)
- Git

## 🖥️ Demo
![demo](./images/HomeView.png)
![demo](./images/LoginView.png)
![demo](./images/PatientDoctorsView.png)
![demo](./images/AdminHomeView.png)

- Repository: https://github.com/MajloszIS/MedicalCenter

---

## Instalacja i konfiguracja
 
### Windows
 
1. Zainstaluj [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
2. Sklonuj repozytorium:
```bash
git clone https://github.com/MajloszIS/MedicalCenter
cd MedicalCenter
```
 
3. Przejdź do folderu projektu:
```bash
cd MedicalCenter
```
 
4. Zastosuj migracje (LocalDB jest domyślnie dostępny na Windows):
```bash
dotnet ef database update
```
 
5. Uruchom aplikację:
```bash
dotnet run
```
 
6. Otwórz przeglądarkę pod adresem `https://localhost:<port>`

---

## Domyślne konta (seed data)
 
| Email | Hasło | Rola |
|-------|-------|------|
| admin@medical.pl | admin123 | Admin |
| doktor@medical.pl | doktor123 | Doctor |
| pacjent@medical.pl | pacjent123 | Patient |
 
## Dokumentacja API
 
Po uruchomieniu aplikacji dokumentacja API dostępna jest pod adresem:
```
https://localhost:<port>/swagger
```
 
## TODO
 
- [ ] Rozbudowa API — więcej endpointów dla wizyt i pacjentów
- [ ] Frontend/CSS — poprawa wyglądu aplikacji
- [ ] Przesyłanie plików — np. zdjęcie profilowe lekarza
- [ ] Zewnętrzne API — np. prognoza pogody
- [ ] Generowanie dokumentów PDF — np. recepta
- [ ] Testy automatyczne — pokrycie głównej logiki aplikacji
- [ ] Wdrożenie na produkcji
