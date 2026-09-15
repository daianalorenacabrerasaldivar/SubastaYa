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
                    UsuarioId = usuarios[0].Id, // vendedor@test.com — cobró $7.500 por Bicicleta Fixie
                    SaldoTotal = 7500,
                    SaldoRetenido = 0,
                    SaldoDisponible = 7500
                },
                new Billetera
                {
                    UsuarioId = usuarios[1].Id, // comprador1@test.com — $45.000 retenidos (postor líder iPhone)
                    SaldoTotal = 150000,
                    SaldoRetenido = 45000,
                    SaldoDisponible = 105000
                },
                new Billetera
                {
                    UsuarioId = usuarios[2].Id, // comprador2@test.com — pagó $7.500 Bicicleta; $12.000 retenidos Charizard
                    SaldoTotal = 192500,
                    SaldoRetenido = 12000,
                    SaldoDisponible = 180500
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

            var subastas = new List<Subasta>
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

            subastas[4].MarcarDesierta(ahora);

            return subastas;
        }

        private static async Task SeedPujasYTransacciones(
            ApplicationDbContext context,
            List<Usuario> usuarios,
            List<Subasta> subastas,
            CancellationToken cancellationToken)
        {
            var billeteras = context.Billeteras.ToList();
            var comprador1 = usuarios[1];
            var comprador2 = usuarios[2];
            var sinfondos  = usuarios[3];
            var billetera1        = billeteras.First(b => b.UsuarioId == comprador1.Id);
            var billetera2        = billeteras.First(b => b.UsuarioId == comprador2.Id);
            var billeteraVendedor = billeteras.First(b => b.UsuarioId == usuarios[0].Id);
            var billeteraSinfondos = billeteras.First(b => b.UsuarioId == sinfondos.Id);

            var ahora = DateTime.UtcNow;
            var pujas = new List<Puja>();
            var transacciones = new List<TransaccionLedger>();

            var subastaActiva        = context.Subastas.FirstOrDefault(s => s.Titulo.Contains("iPhone 15 Pro Max"));
            var subastaCharizard     = context.Subastas.FirstOrDefault(s => s.Titulo.Contains("Charizard"));
            var subastaVencidaGanador = context.Subastas.FirstOrDefault(s => s.Titulo.Contains("Bicicleta Fixie"));

            // ── Depósitos iniciales ──────────────────────────────────────────────────
            transacciones.Add(new TransaccionLedger
            {
                BilleteraId = billetera1.Id,
                Tipo = TipoTransaccion.Deposito,
                Monto = 150000,
                Fecha = ahora.AddDays(-30),
                SubastaId = null
            });

            transacciones.Add(new TransaccionLedger
            {
                BilleteraId = billetera2.Id,
                Tipo = TipoTransaccion.Deposito,
                Monto = 200000,
                Fecha = ahora.AddDays(-20),
                SubastaId = null
            });

            transacciones.Add(new TransaccionLedger
            {
                BilleteraId = billeteraSinfondos.Id,
                Tipo = TipoTransaccion.Deposito,
                Monto = 500,
                Fecha = ahora.AddDays(-5),
                SubastaId = null
            });

            // ── iPhone 15 Pro Max — 3 pujas con ciclo completo Retención/Liberación ─
            if (subastaActiva != null)
            {
                // Puja 1 — comprador1 $35.000
                pujas.Add(new Puja
                {
                    SubastaId = subastaActiva.Id,
                    CompradorId = comprador1.Id,
                    Monto = 35000,
                    FechaPuja = ahora.AddMinutes(-5)
                });
                transacciones.Add(new TransaccionLedger
                {
                    BilleteraId = billetera1.Id,
                    Tipo = TipoTransaccion.Retencion,
                    Monto = 35000,
                    Fecha = ahora.AddMinutes(-5),
                    SubastaId = subastaActiva.Id
                });

                // Puja 2 — comprador2 $40.000  →  libera retención de comprador1
                pujas.Add(new Puja
                {
                    SubastaId = subastaActiva.Id,
                    CompradorId = comprador2.Id,
                    Monto = 40000,
                    FechaPuja = ahora.AddMinutes(-3)
                });
                transacciones.Add(new TransaccionLedger
                {
                    BilleteraId = billetera1.Id,
                    Tipo = TipoTransaccion.Liberacion,
                    Monto = 35000,
                    Fecha = ahora.AddMinutes(-3),
                    SubastaId = null
                });
                transacciones.Add(new TransaccionLedger
                {
                    BilleteraId = billetera2.Id,
                    Tipo = TipoTransaccion.Retencion,
                    Monto = 40000,
                    Fecha = ahora.AddMinutes(-3),
                    SubastaId = subastaActiva.Id
                });

                // Puja 3 (líder actual) — comprador1 $45.000  →  libera retención de comprador2
                pujas.Add(new Puja
                {
                    SubastaId = subastaActiva.Id,
                    CompradorId = comprador1.Id,
                    Monto = 45000,
                    FechaPuja = ahora.AddMinutes(-1)
                });
                transacciones.Add(new TransaccionLedger
                {
                    BilleteraId = billetera2.Id,
                    Tipo = TipoTransaccion.Liberacion,
                    Monto = 40000,
                    Fecha = ahora.AddMinutes(-1),
                    SubastaId = null
                });
                transacciones.Add(new TransaccionLedger
                {
                    BilleteraId = billetera1.Id,
                    Tipo = TipoTransaccion.Retencion,
                    Monto = 45000,
                    Fecha = ahora.AddMinutes(-1),
                    SubastaId = subastaActiva.Id
                });
            }

            // ── Charizard — 1 puja activa de comprador2 (zona crítica < 2 min) ──────
            if (subastaCharizard != null)
            {
                pujas.Add(new Puja
                {
                    SubastaId = subastaCharizard.Id,
                    CompradorId = comprador2.Id,
                    Monto = 12000,
                    FechaPuja = ahora.AddSeconds(-120)
                });
                transacciones.Add(new TransaccionLedger
                {
                    BilleteraId = billetera2.Id,
                    Tipo = TipoTransaccion.Retencion,
                    Monto = 12000,
                    Fecha = ahora.AddSeconds(-120),
                    SubastaId = subastaCharizard.Id
                });
            }

            // ── Bicicleta Fixie — puja ganadora + Retención → Pago + Cobro vendedor ─
            if (subastaVencidaGanador != null)
            {
                var pujaGanadora = new Puja
                {
                    SubastaId = subastaVencidaGanador.Id,
                    CompradorId = comprador2.Id,
                    Monto = 7500,
                    FechaPuja = ahora.AddHours(-2)
                };
                subastaVencidaGanador.Pujas.Add(pujaGanadora);
                subastaVencidaGanador.Finalizar(ahora);

                // Retención al momento de la puja
                transacciones.Add(new TransaccionLedger
                {
                    BilleteraId = billetera2.Id,
                    Tipo = TipoTransaccion.Retencion,
                    Monto = 7500,
                    Fecha = ahora.AddHours(-2),
                    SubastaId = subastaVencidaGanador.Id
                });

                // Adjudicación: la retención se convierte en Pago (reduce SaldoTotal del comprador)
                transacciones.Add(new TransaccionLedger
                {
                    BilleteraId = billetera2.Id,
                    Tipo = TipoTransaccion.Pago,
                    Monto = 7500,
                    Fecha = ahora.AddMinutes(-30),
                    SubastaId = subastaVencidaGanador.Id
                });

                // Cobro al vendedor
                transacciones.Add(new TransaccionLedger
                {
                    BilleteraId = billeteraVendedor.Id,
                    Tipo = TipoTransaccion.Cobro,
                    Monto = 7500,
                    Fecha = ahora.AddMinutes(-30),
                    SubastaId = subastaVencidaGanador.Id
                });
            }

            context.Pujas.AddRange(pujas);
            context.TransaccionesLedger.AddRange(transacciones);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
