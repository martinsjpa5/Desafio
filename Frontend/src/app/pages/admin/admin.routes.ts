import { Routes } from "@angular/router";
import { ListarComprasAdminComponent } from "./compras/listar-compras-admin.component";
import { ListarProdutosAdminComponent } from "./produtos/listar-produtos-admin.component";
import { ListarRelatorioAdminComponent } from "./relatorio/listar-relatorio-admin.component";

export const ADMIN_ROUTES: Routes = [
  {
    path: 'compras',
    component: ListarComprasAdminComponent
  },
  {
    path: 'produtos',
    component: ListarProdutosAdminComponent
  },
  {
    path: 'relatorio',
    component: ListarRelatorioAdminComponent
  },
];