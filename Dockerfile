# contruir todo
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS dependencias
WORKDIR /api
COPY back-2-trimestre-2-daw.csproj ./ 
# dependencias preparadas
RUN dotnet restore 
COPY . ./
RUN dotnet publish -o salida 
# -o es el output, la ruta donde irán los archivos

# ejecutarlo
FROM mcr.microsoft.com/dotnet/aspnet:8.0
ENV ASPNETCORE_ENVIRONMENT=Development
# variable de entorno para lanzar el aplicativo en modo desarrollo, por lo que el swagger tb será visible
 # escucha en puerto 8080 (el de la derecha al ejecutarlo) learn.microsoft.com/en-us/dotnet/core/compatibility/containers/8.0/aspnet-port
WORKDIR /api
COPY --from=dependencias /api/salida . 
# copia los archivos del dir salida de contruccion
ENTRYPOINT ["dotnet", "back-2-trimestre-2-daw.dll"]
