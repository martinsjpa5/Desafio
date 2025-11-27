import { Component, OnInit } from '@angular/core';
import { CompraListarResponse } from '../../../domain/models/compra-listar.model';
import { CompraService } from '../../../domain/services/compra.service';
import { CommonModule } from '@angular/common';
import { LoadingService } from '../../../core/services/loading.service';

@Component({
    standalone: true,
    imports: [CommonModule],
    selector: 'app-listar-compras-usuario',
    templateUrl: './listar-compras-usuario.component.html'
})
export class ListarComprasUsuarioComponent implements OnInit {
    compras: CompraListarResponse[] = [];
    errorMessage = '';

    constructor(private compraService: CompraService, private loadingService: LoadingService) { }

    ngOnInit() {
        this.buscarCompras();
    }

    async buscarCompras() {
        this.loadingService.show();
        this.errorMessage = '';
        try {
            let response = await this.compraService.listarComprasUsuario();
            this.compras = response.data;

        }
        catch (error: any) {
            if (error?.error?.erros) {
                this.errorMessage = error.error.erros.join('<br>');
            }
            else {
                this.errorMessage = 'Erro ao carregar compras.';
            }
        }
        finally {
            this.loadingService.hide();
        }
    }
}
