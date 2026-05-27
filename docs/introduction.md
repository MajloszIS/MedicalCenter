# MedicalCenter

Aplikacja webowa centrum medycznego zbudowana w ASP.NET Core MVC (.NET 9)
z modułem sprzedaży leków i obsługą dostaw.

## Funkcje

- Rejestracja i logowanie (klasyczne + Google OAuth)
- Panel pacjenta: umawianie wizyt, karta medyczna, recepty, zamówienia
- Panel lekarza: zarządzanie wizytami, wystawianie diagnoz, recept, zwolnień
- Panel administratora: zarządzanie użytkownikami, lekami, departamentami
- Panel kuriera: realizacja dostaw zamówień
- Sklep z lekami z płatnościami Stripe
- Generowanie recept jako PDF (QuestPDF)
- REST API z dokumentacją Swagger

## Stack technologiczny

- ASP.NET Core MVC 9.0
- Entity Framework Core 9
- SQL Server (Azure SQL Database na produkcji)
- Bootstrap 5, Razor Views
- Stripe (płatności)
- Google OAuth (logowanie)
- QuestPDF (generowanie dokumentów)

## Repozytorium

Aplikacja produkcyjna: [[link do Repozytorium](https://github.com/MajloszIS/MedicalCenter)]

## Więcej informacji

- [README projektu](/_site/README.html) — pełna instrukcja instalacji i konfiguracji