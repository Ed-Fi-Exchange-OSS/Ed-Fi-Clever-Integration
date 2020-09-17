
version 1

https://localhost:44354/weatherforecast
https://localhost:44354/api/endpoint/orgs
https://localhost:44354/api/endpoint/orgs?limit=100&offset=100
https://localhost:44354/api/endpoint/orgs?limit=100&offset=0
https://localhost:44354/api/endpoint/users?limit=10&offset=0

https://localhost:44354/api/endpoint/courses?limit=10&offset=0
https://localhost:44354/api/endpoint/classes?limit=10&offset=0
https://localhost:44354/api/endpoint/enrollments?limit=10&offset=0
https://localhost:44354/api/endpoint/demographics?limit=10&offset=0

dotnet ef dbcontext scaffold "Host=my_host;Database=my_db;Username=my_user;Password=my_pw" Npgsql.EntityFrameworkCore.PostgreSQL
dotnet ef dbcontext scaffold "Host=192.168.1.72;Database=edfipost;Username=postgres;Password=gatito01" Npgsql.EntityFrameworkCore.PostgreSQL --schema onerosterv11

Scaffold-DbContext "Host=192.168.1.72;Database=edfipost;Username=postgres;Password=gatito01" Npgsql.EntityFrameworkCore.PostgreSQL -OutputDir EntityFramework -schema onerosterv11 -UseDataBaseNames 
Scaffold-DbContext "Host=192.168.1.72;Database=edfipost;Username=postgres;Password=gatito01" Npgsql.EntityFrameworkCore.PostgreSQL -OutputDir EntityFramework -schema onerosterv11 -UseDataBaseNames  -Force
 [JsonIgnore]
--schema onerosterv11
enrollments

https://nearshoredevs.landingzone.dev/onerosterapi/campus/oneroster/go/ims/oneroster/v1p1