using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CleanArchitecture.Domain.Entities
{
    public class Producto : EntidadBase
    {
        [Required, MaxLength(200)]
        public string Nombre { get; set; } = default!;

        [MaxLength(1000)]
        public string? Descripcion { get; set; }

        // relaciones
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; } = default!;
        public int UnidadMedidaId { get; set; }
        public UnidadMedida UnidadMedida { get; set; } = default!;
        [Column(TypeName = "decimal(18,2)")]
        public decimal Costo { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioVenta { get; set; }

    }
}
