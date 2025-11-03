# SBStore – Platforma E-Commerce w ASP.NET Core

**SBStore** to w pełni funkcjonalna aplikacja e-commerce zbudowana w technologii **ASP.NET Core** i **Entity Framework Core**.

---

## Funkcjonalności

- 👤 Rejestracja i logowanie użytkowników (z rolami: **Customer**, **Company**, **Employee**, **Admin**)  
- 📦 Zarządzanie produktami (dodawanie, edycja, usuwanie, przypisanie do kategorii)  
- 🏢 Obsługa firm i kategorii produktów  
- 🛍 Koszyk zakupowy oraz proces składania zamówienia  
- 💳 Integracja z systemem płatności **Stripe**  
- 📘 Architektura z wykorzystaniem wzorców **Repository Pattern**, **Unit of Work**, **Dependency Injection**  
- ✅ Walidacja danych po stronie klienta i serwera  
- 🔐 Autoryzacja i uwierzytelnianie przez **ASP.NET Core Identity**

---

## Architektura projektu

Projekt został oparty na klasycznym podziale warstw:
- Modele danych: `Product`, `Category`, `Company`, `ShoppingCart`, `OrderHeader`, `OrderDetail`, `AppUser` |
- Kontekst bazy danych `AppDbContext`, migracje, seedowanie danych |
- Wzorzec **Repository** + **Unit of Work**, logika CRUD dla modeli |
- Klasy pomocnicze: `SD` (stałe aplikacji), `StripeSettings` (konfiguracja płatności) |
- Warstwa prezentacji (Controllers, Views, Razor Pages, wwwroot) |

---

## Opis przepływu danych

1. Użytkownik wykonuje akcję w interfejsie (np. „Dodaj produkt”).  
2. Controller odbiera żądanie i korzysta z `UnitOfWork`, aby odwołać się do odpowiedniego repozytorium.  
3. Repository komunikuje się z `AppDbContext`, który zarządza encjami EF Core.  
4. AppDbContext tłumaczy operacje C# na zapytania SQL i zapisuje dane w bazie.  
5. Wynik jest zwracany do widoku i prezentowany użytkownikowi.

---
### Walidacja i bezpieczeństwo
- Walidacja po stronie klienta i serwera  
- Uwierzytelnianie przez **ASP.NET Core Identity**  
- Autoryzacja na poziomie ról  


## Struktura projektu

- Models -	Modele danych: Product, Category, Company, ShoppingCart, OrderHeader, OrderDetail, AppUser
- DataAccess -	AppDbContext, konfiguracja DbSet<T>, seedowanie danych
- Repository -	Generyczne repozytorium Repository<T>, konkretne repozytoria (np. CompanyRepository)
- UnitOfWork -	Klasa UnitOfWork implementująca interfejs IUnitOfWork, koordynując repozytoria
- Utility -	Klasy pomocnicze: SD (stałe aplikacji), StripeSettings (ustawienia płatności)
- Controllers & Views -	Logika aplikacji, widoki Razor, akcje użytkownika i administratora

---

## Płatności i statusy

- Integracja z **Stripe API** (ustawienia w `StripeSettings`)  
- **Statusy zamówień:** Pending, Approved, Processing, Shipped, Cancelled, Refunded  
- **Statusy płatności:** Pending, Approved, DelayedPayment, Rejected  

---

## Walidacja i bezpieczeństwo

- Walidacja po stronie **klienta (JavaScript)** i **serwera (DataAnnotations)**  
- Uwierzytelnianie i role przez **ASP.NET Core Identity**  
- Bezpieczne przetwarzanie płatności (Stripe)  

---

## Technologie

- **.NET 8.0 / ASP.NET Core MVC**  
- **Entity Framework Core (Code-First)**  
- **SQL Server** (lub inny provider EF)  
- **ASP.NET Core Identity** – logowanie i autoryzacja  
- **Repository Pattern** + **Unit of Work**  
- **Stripe API** – obsługa płatności  

**Opis przepływu:**
1. Użytkownik wykonuje akcję w interfejsie (np. „Dodaj produkt”).  
2. Controller odbiera żądanie i korzysta z `UnitOfWork`, aby odwołać się do konkretnego repozytorium.  
3. `Repository` komunikuje się z `AppDbContext`, który zarządza encjami EF Core.  
4. `AppDbContext` tłumaczy operacje C# na SQL i zapisuje dane w bazie.  
5. Odpowiedź jest zwracana z powrotem do kontrolera i wyświetlana użytkownikowi.

## Instalacja i uruchomienie  
Aby uruchomić projekt lokalnie, wykonaj poniższe kroki:
- Sklonuj repozytorium
- zaktualizuj połączenie z bazą danych w pliku appsettings.json ( "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SBStore;Trusted_Connection=True;")
- wykonaj migrację bazy danych poleceniem update-database w konsoli
- uruchom poleceniem dotnet run
- 
## Autor
Szymon Bywalec
Na podstawie kursu .NET Core - The Complete Guide (by Bhrugen Patel)

<img width="1056" height="786" alt="image" src="https://github.com/user-attachments/assets/30e64176-48dc-4b54-84a2-006859d697cd" />
<img width="1039" height="701" alt="image" src="https://github.com/user-attachments/assets/95001da1-a0dd-4523-a706-5a97b9303756" />
<img width="1055" height="703" alt="image" src="https://github.com/user-attachments/assets/fc63cca8-689d-4d10-aa08-9a748b409dc5" />
<img width="1060" height="700" alt="image" src="https://github.com/user-attachments/assets/16950106-6213-415e-bfaf-1d0dae650e0f" />
<img width="1057" height="698" alt="image" src="https://github.com/user-attachments/assets/2eb63b71-daad-4fa4-9a6e-c7bc8ac367c9" />
<img width="1058" height="703" alt="image" src="https://github.com/user-attachments/assets/6bb312da-4d09-43f9-9334-f91c2723b88d" />
<img width="1048" height="702" alt="image" src="https://github.com/user-attachments/assets/4ae5ec6b-fc57-4ccd-9b6b-11e64d397e17" />
<img width="1047" height="699" alt="image" src="https://github.com/user-attachments/assets/a166ee6c-03fe-4847-80f9-b0c8db9f471b" />
<img width="1057" height="700" alt="image" src="https://github.com/user-attachments/assets/b4df0ef8-f83e-4b58-ac21-d10d40fec944" />
<img width="1057" height="785" alt="image" src="https://github.com/user-attachments/assets/dfcdb116-bc05-47f4-8115-fcd8f4bfd096" />
<img width="1074" height="743" alt="image" src="https://github.com/user-attachments/assets/6ae07355-a574-4d20-90e4-4c456c33f063" />
