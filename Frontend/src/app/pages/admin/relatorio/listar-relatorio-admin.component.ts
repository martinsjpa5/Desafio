import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
  AbstractControl,
  ValidationErrors
} from '@angular/forms';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { PoButtonModule, PoFieldModule, PoTableModule } from '@po-ui/ng-components';

import { RelatorioResponse } from '../../../domain/models/relatorio-response.model';
import { RelatorioService } from '../../../domain/services/relatorio.service';
import { LoadingService } from '../../../core/services/loading.service';

@Component({
  selector: 'app-listar-relatorio-admin',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, NgbModule, PoFieldModule, PoButtonModule, PoTableModule],
  templateUrl: './listar-relatorio-admin.component.html',
  styleUrls: ['./listar-relatorio-admin.component.scss']
})
export class ListarRelatorioAdminComponent implements OnInit {

  relatorios: RelatorioResponse[] = [];
  errorMessage: string = '';
  successMessage: string = '';

  form!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private relatorioService: RelatorioService,
    private loadingService: LoadingService
  ) {}

  ngOnInit(): void {
    // Datas padrão: 1º dia do ano atual e hoje
    const startDefault = this.toDateInputValue(new Date(new Date().getFullYear(), 0, 1));
    const endDefault = this.toDateInputValue(new Date());

    this.form = this.fb.group(
      {
        start: [startDefault, [Validators.required]],
        end: [endDefault, [Validators.required]]
      },
      { validators: [this.dateRangeValidator] }
    );

    this.CarregarRelatorios();
  }

  // =========================
  // Helpers para input type="date"
  // =========================

  /** Converte Date -> 'yyyy-MM-dd' (para o input date aceitar) */
  private toDateInputValue(date: Date): string {
    const y = date.getFullYear();
    const m = String(date.getMonth() + 1).padStart(2, '0');
    const d = String(date.getDate()).padStart(2, '0');
    return `${y}-${m}-${d}`;
  }

  /** Converte 'yyyy-MM-dd' -> Date (local, sem “puxar” pro dia anterior por UTC) */
  private fromDateInputValue(value: string): Date {
    const [y, m, d] = value.split('-').map(Number);
    return new Date(y, m - 1, d);
  }

  // =========================
  // Validator do intervalo
  // =========================

  private dateRangeValidator = (group: AbstractControl): ValidationErrors | null => {
    const start = group.get('start')?.value as string;
    const end = group.get('end')?.value as string;

    if (!start || !end) return null; // required já cobre

    const startDate = this.fromDateInputValue(start);
    const endDate = this.fromDateInputValue(end);

    if (endDate < startDate) {
      return { dateRangeInvalid: true };
    }

    return null;
  };

  get f() {
    return this.form.controls;
  }

  // =========================
  // Ações
  // =========================

  async SolicitarRelatorio() {
    this.errorMessage = '';
    this.successMessage = '';

    // força validações
    this.form.markAllAsTouched();

    if (this.form.invalid) {
      // Mensagem “tipo login”: mais amigável
      if (this.f['start']?.errors?.['required'] || this.f['end']?.errors?.['required']) {
        this.errorMessage = 'Preencha ambas as datas.';
      } else if (this.form.errors?.['dateRangeInvalid']) {
        this.errorMessage = 'A data final não pode ser menor que a inicial.';
      } else {
        this.errorMessage = 'Verifique os campos.';
      }
      return;
    }

    const startDate = this.fromDateInputValue(this.f['start'].value);
    const endDate = this.fromDateInputValue(this.f['end'].value);

    this.loadingService.show();

    try {
      await this.relatorioService.Solicitar(startDate, endDate);

      this.relatorios = [];

      setTimeout(() => {
        this.CarregarRelatorios();
      }, 5000);

      this.successMessage = 'Relatório solicitado com sucesso. Em breve ele estará disponível.';
    } catch (error: any) {
      if (error?.error?.erros) {
        this.errorMessage = error.error.erros.join('<br>');
      } else {
        this.errorMessage = 'Erro ao solicitar relatório.';
      }
    } finally {
      this.loadingService.hide();
    }
  }

  async CarregarRelatorios() {
    this.loadingService.show();
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
