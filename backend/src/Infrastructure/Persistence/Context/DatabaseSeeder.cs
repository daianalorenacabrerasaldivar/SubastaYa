using System.Security.Cryptography;
using System.Text;
using Domain.Entity;
using Domain.Enum;

namespace Infrastructure.Persistence.Context
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context, CancellationToken cancellationToken = default)
        {
            // No ejecutar si hay datos
            if (context.Usuarios.Any() || context.Categorias.Any() || context.Subastas.Any())
                return;

            try
            {
                // 1. Crear categorías
                var categorias = SeedCategorias();
                context.Categorias.AddRange(categorias);
                await context.SaveChangesAsync(cancellationToken);

                // 2. Crear usuarios y billeteras
                var usuarios = SeedUsuariosYBilleteras();
                context.Usuarios.AddRange(usuarios);
                await context.SaveChangesAsync(cancellationToken);

                // 3. Crear billeteras
                var billeteras = SeedBilleteras(usuarios);
                context.Billeteras.AddRange(billeteras);
                await context.SaveChangesAsync(cancellationToken);

                // 4. Crear subastas
                var subastas = SeedSubastas(usuarios, categorias);
                context.Subastas.AddRange(subastas);
                await context.SaveChangesAsync(cancellationToken);

                // 5. Crear pujas y transacciones del ledger
                await SeedPujasYTransacciones(context, usuarios, subastas, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al popular la base de datos", ex);
            }
        }

        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private static List<Categoria> SeedCategorias()
        {
            return new List<Categoria>
            {
                new Categoria { Nombre = "Tecnología", UrlIcono = "https://example.com/tech.png" },
                new Categoria { Nombre = "Coleccionables", UrlIcono = "https://example.com/collect.png" },
                new Categoria { Nombre = "Indumentaria", UrlIcono = "https://example.com/clothes.png" },
                new Categoria { Nombre = "Vehículos", UrlIcono = "https://example.com/vehicles.png" }
            };
        }

        private static List<Usuario> SeedUsuariosYBilleteras()
        {
            var passwordHash = HashPassword("Password123!");

            return new List<Usuario>
            {
                new Usuario
                {
                    Email = "vendedor@test.com",
                    Nombre = "Vendedor Test",
                    PasswordHash = passwordHash,
                    FechaRegistro = DateTime.UtcNow
                },
                new Usuario
                {
                    Email = "comprador1@test.com",
                    Nombre = "Comprador Uno",
                    PasswordHash = passwordHash,
                    FechaRegistro = DateTime.UtcNow
                },
                new Usuario
                {
                    Email = "comprador2@test.com",
                    Nombre = "Comprador Dos",
                    PasswordHash = passwordHash,
                    FechaRegistro = DateTime.UtcNow
                },
                new Usuario
                {
                    Email = "sinfondos@test.com",
                    Nombre = "Sin Fondos",
                    PasswordHash = passwordHash,
                    FechaRegistro = DateTime.UtcNow
                }
            };
        }

        private static List<Billetera> SeedBilleteras(List<Usuario> usuarios)
        {
            return new List<Billetera>
            {
                new Billetera
                {
                    UsuarioId = usuarios[0].Id, // vendedor@test.com
                    SaldoTotal = 0,
                    SaldoRetenido = 0,
                    SaldoDisponible = 0
                },
                new Billetera
                {
                    UsuarioId = usuarios[1].Id, // comprador1@test.com
                    SaldoTotal = 150000,
                    SaldoRetenido = 45000,
                    SaldoDisponible = 105000
                },
                new Billetera
                {
                    UsuarioId = usuarios[2].Id, // comprador2@test.com
                    SaldoTotal = 200000,
                    SaldoRetenido = 0,
                    SaldoDisponible = 200000
                },
                new Billetera
                {
                    UsuarioId = usuarios[3].Id, // sinfondos@test.com
                    SaldoTotal = 500,
                    SaldoRetenido = 0,
                    SaldoDisponible = 500
                }
            };
        }

        private static List<Subasta> SeedSubastas(List<Usuario> usuarios, List<Categoria> categorias)
        {
            var ahora = DateTime.UtcNow;

            return new List<Subasta>
            {
                // 1. Activa estándar: Cierra en 20-30 min (con 2 pujas previas cargadas; líder $45.000)
                Subasta.Create(
                    vendedorId: usuarios[0].Id,
                    categoriaId: categorias[0].Id,
                    titulo: "iPhone 15 Pro Max - Activa Estándar",
                    descripcion: "iPhone 15 Pro Max en perfecto estado, con accesorios originales.",
                    urlImagen: "https://example.com/iphone15.png",
                    precioBase: 30000,
                    incrementoMinimo: 5000,
                    fechaInicio: ahora.AddMinutes(-5),
                    fechaFin: ahora.AddMinutes(25)
                ),
                // 2. Activa crítica: Cierra en menos de 2 min (para probar alerta visual y extensión anti-sniping)
                Subasta.Create(
                    vendedorId: usuarios[0].Id,
                    categoriaId: categorias[1].Id,
                    titulo: "Tarjeta Pokémon Charizard 1ª Edición - Crítica",
                    descripcion: "Tarjeta de colección rara, estado PSA 9.",
                    urlImagen: "https://example.com/charizard.png",
                    precioBase: 10000,
                    incrementoMinimo: 2000,
                    fechaInicio: ahora.AddMinutes(-3),
                    fechaFin: ahora.AddSeconds(90)
                ),
                // 3. Próxima: Inicio programado a +24 hs (pujas bloqueadas)
                Subasta.Create(
                    vendedorId: usuarios[0].Id,
                    categoriaId: categorias[2].Id,
                    titulo: "Chaqueta Vintage Dior - Próxima",
                    descripcion: "Chaqueta de diseñador vintage en excelentes condiciones.",
                    urlImagen: "https://example.com/dior.png",
                    precioBase: 8000,
                    incrementoMinimo: 1000,
                    fechaInicio: ahora.AddHours(24),
                    fechaFin: ahora.AddHours(48)
                ),
                // 4. Vencida con ganador: Fecha fin pasada + puja ganadora
                Subasta.Create(
                    vendedorId: usuarios[0].Id,
                    categoriaId: categorias[3].Id,
                    titulo: "Bicicleta Fixie Vintage - Vencida Ganador",
                    descripcion: "Bicicleta de carrera restaurada, lista para usar.",
                    urlImagen: "https://example.com/fixie.png",
                    precioBase: 5000,
                    incrementoMinimo: 500,
                    fechaInicio: ahora.AddHours(-4),
                    fechaFin: ahora.AddMinutes(-30)
                ),
                // 5. Vencida desierta: Fecha fin pasada sin pujas
                Subasta.Create(
                    vendedorId: usuarios[0].Id,
                    categoriaId: categorias[0].Id,
                    titulo: "Funda Para Laptop Desierta",
                    descripcion: "Funda protectora de neopreno para laptop 15 pulgadas.",
                    urlImagen: "https://example.com/funda.png",
                    precioBase: 2000,
                    incrementoMinimo: 500,
                    fechaInicio: ahora.AddHours(-3),
                    fechaFin: ahora.AddMinutes(-45)
                )
            };
        }

        private static async Task SeedPujasYTransacciones(
            ApplicationDbContext context,
            List<Usuario> usuarios,
            List<Subasta> subastas,
            CancellationToken cancellationToken)
        {
            var billeteras = context.Billeteras.ToList();
            var comprador1 = usuarios[1]; // comprador1@test.com
            var comprador2 = usuarios[2]; // comprador2@test.com
            var billetera1 = billeteras.First(b => b.UsuarioId == comprador1.Id);
            var billetera2 = billeteras.First(b => b.UsuarioId == comprador2.Id);

            var ahora = DateTime.UtcNow;
            var pujas = new List<Puja>();
            var transacciones = new List<TransaccionLedger>();

            // Subastas reales (sin IDs aún, serán asignados tras SaveChanges)
            var subastaActiva = context.Subastas.FirstOrDefault(s => 
                s.Titulo.Contains("iPhone 15 Pro Max"));
            var subastaVencidaGanador = context.Subastas.FirstOrDefault(s => 
                s.Titulo.Contains("Bicicleta Fixie Vintage"));

            // 1. Pujas en la subasta activa estándar
            if (subastaActiva != null)
            {
                // Primera puja: Comprador 1 por $35.000
                var puja1 = new Puja
                {
                    SubastaId = subastaActiva.Id,
                    CompradorId = comprador1.Id,
                    Monto = 35000,
                    FechaPuja = ahora.AddMinutes(-5)
                };
                pujas.Add(puja1);

                // Segunda puja: Comprador 2 por $40.000
                var puja2 = new Puja
                {
                    SubastaId = subastaActiva.Id,
                    CompradorId = comprador2.Id,
                    Monto = 40000,
                    FechaPuja = ahora.AddMinutes(-3)
                };
                pujas.Add(puja2);

                // Tercera puja (actual): Comprador 1 por $45.000
                var puja3 = new Puja
                {
                    SubastaId = subastaActiva.Id,
                    CompradorId = comprador1.Id,
                    Monto = 45000,
                    FechaPuja = ahora.AddMinutes(-1)
                };
                pujas.Add(puja3);

                // Transacciones en el ledger (auditoría de movimientos)
                // 1. Depósito inicial de comprador1 (simulado, históricamente)
                transacciones.Add(new TransaccionLedger
                {
                    BilleteraId = billetera1.Id,
                    Tipo = TipoTransaccion.Deposito,
                    Monto = 150000,
                    Fecha = ahora.AddDays(-30),
                    SubastaId = null
                });

                // 2. Retención por primera puja de $35.000
                transacciones.Add(new TransaccionLedger
                {
                    BilleteraId = billetera1.Id,
                    Tipo = TipoTransaccion.Retencion,
                    Monto = 35000,
                    Fecha = ahora.AddMinutes(-5),
                    SubastaId = subastaActiva.Id
                });

                // 3. Liberación de retención anterior ($35.000)
                transacciones.Add(new TransaccionLedger
                {
                    BilleteraId = billetera1.Id,
                    Tipo = TipoTransaccion.Liberacion,
                    Monto = 35000,
                    Fecha = ahora.AddMinutes(-4),
                    SubastaId = null
                });

                // 4. Retención por segunda puja de $40.000 de comprador2
                transacciones.Add(new TransaccionLedger
                {
                    BilleteraId = billetera2.Id,
                    Tipo = TipoTransaccion.Retencion,
                    Monto = 40000,
                    Fecha = ahora.AddMinutes(-3),
                    SubastaId = subastaActiva.Id
                });

                // 5. Liberación de retención de comprador2 ($40.000)
                transacciones.Add(new TransaccionLedger
                {
                    BilleteraId = billetera2.Id,
                    Tipo = TipoTransaccion.Liberacion,
                    Monto = 40000,
                    Fecha = ahora.AddMinutes(-2),
                    SubastaId = null
                });

                // 6. Retención final por tercera puja de $45.000 de comprador1
                transacciones.Add(new TransaccionLedger
                {
                    BilleteraId = billetera1.Id,
                    Tipo = TipoTransaccion.Retencion,
                    Monto = 45000,
                    Fecha = ahora.AddMinutes(-1),
                    SubastaId = subastaActiva.Id
                });

                // Depósito inicial de comprador2
                transacciones.Add(new TransaccionLedger
                {
                    BilleteraId = billetera2.Id,
                    Tipo = TipoTransaccion.Deposito,
                    Monto = 200000,
                    Fecha = ahora.AddDays(-20),
                    SubastaId = null
                });
            }

            // 2. Puja ganadora en la subasta vencida
            if (subastaVencidaGanador != null)
            {
                var pujaGanadora = new Puja
                {
                    SubastaId = subastaVencidaGanador.Id,
                    CompradorId = comprador2.Id,
                    Monto = 7500,
                    FechaPuja = ahora.AddHours(-2)
                };
                pujas.Add(pujaGanadora);

                // Transacción: Pago de la subasta ganada
                transacciones.Add(new TransaccionLedger
                {
                    BilleteraId = billetera2.Id,
                    Tipo = TipoTransaccion.Pago,
                    Monto = 7500,
                    Fecha = ahora.AddHours(-1),
                    SubastaId = subastaVencidaGanador.Id
                });

                // Transacción: Cobro al vendedor
                var billeteraVendedor = billeteras.First(b => b.UsuarioId == usuarios[0].Id);
                transacciones.Add(new TransaccionLedger
                {
                    BilleteraId = billeteraVendedor.Id,
                    Tipo = TipoTransaccion.Cobro,
                    Monto = 7500,
                    Fecha = ahora.AddHours(-1),
                    SubastaId = subastaVencidaGanador.Id
                });
            }

            // Agregar pujas y transacciones a la base de datos
            context.Pujas.AddRange(pujas);
            context.TransaccionesLedger.AddRange(transacciones);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
