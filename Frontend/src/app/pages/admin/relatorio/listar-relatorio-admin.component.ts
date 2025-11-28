import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { RelatorioResponse } from '../../../domain/models/relatorio-response.model';
import { RelatorioService } from '../../../domain/services/relatorio.service';
import { LoadingService } from '../../../core/services/loading.service';
import { PoButtonModule, PoFieldModule, PoTableAction, PoTableModule } from '@po-ui/ng-components';

@Component({
  selector: 'app-listar-relatorio-admin',
  standalone: true,
  imports: [CommonModule, FormsModule, NgbModule, PoFieldModule, PoButtonModule,PoTableModule  ],
  templateUrl: './listar-relatorio-admin.component.html',
  styleUrls: ['./listar-relatorio-admin.component.scss']
})
export class ListarRelatorioAdminComponent implements OnInit {

  relatorios: RelatorioResponse[] = [];
  errorMessage: string = '';

  intervaloData = {
    start: new Date(2025, 0, 1),
    end: new Date()
  };

  constructor(private relatorioService: RelatorioService, private loadingService: LoadingService) { }

  ngOnInit(): void {
    this.CarregarRelatorios();
  }

  async SolicitarRelatorio() {
    if (!this.intervaloData.start || !this.intervaloData.end) {
      this.errorMessage = 'Preencha ambas as datas.';
      return;
    }

    if (this.intervaloData.end < this.intervaloData.start) {
      this.errorMessage = 'A data final não pode ser menor que a inicial.';
      return;
    }

    this.loadingService.show();
    this.errorMessage = '';

    try {
      await this.relatorioService.Solicitar(this.intervaloData.start, this.intervaloData.end);
      this.relatorios = [];
      setTimeout(() => {
        this.CarregarRelatorios();
      }, 5000);

      this.errorMessage = 'Relatório solicitado com sucesso. Em breve ele estará disponível.';
    } catch (error: any) {
      if (error?.error?.erros) {
        this.errorMessage = error.error.erros.join('<br>');
      }
      else {
        this.errorMessage = 'Erro ao solicitar relatório.';
      }
    } finally {
      this.loadingService.hide();
    }
  }

  async CarregarRelatorios() {
    this.loadingService.show()
    try {
      const response = await this.relatorioService.Obter();
      this.relatorios = response.data ?? [];
    } catch (error) {
      this.errorMessage = 'Erro ao carregar relatórios.';
    } finally {
      this.loadingService.hide();
    }
  }

}
