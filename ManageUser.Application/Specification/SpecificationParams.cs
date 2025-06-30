using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageUser.Application.Specification
{
    public abstract class SpecificationParams
    {
        //  encapsula los parámetros comunes de paginación, filtrado y ordenamiento para estandarizar entre consultas.
        //  Al construir una specification, se inyecta SpecificationParams para obtener los parámetros con los valores actuales.

        public string? Search { get; set; }
        public string? Sort { get; set; }
        public int? PageIndex { get; set; }
        private const int MaxPageSize = 50;
        private int? _pageSize = 4;
        public int? PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
        public string? TenantId { get; set; }// Identificador del inquilino (tenant) para filtrar los resultados por inquilino
        public bool OnlyActive { get; set; } = true; // Indica si se deben filtrar solo los resultados activos
    }
}
