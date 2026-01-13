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

import { RelatorioResponse } from '../../../domain/models/relatorio-response.model';
import { RelatorioService } from '../../../domain/services/relatorio.service';
import { LoadingService } from '../../../core/services/loading.service';
import { ToastService } from '../../../core/services/toast.service';
import { ApiErrorHelper } from '../../../core/helpers/api-error.helper';

@Component({
  selector: 'app-listar-relatorio-admin',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, NgbModule],
  templateUrl: './listar-relatorio-admin.component.html',
  styleUrls: ['./listar-relatorio-admin.component.scss']
})
export class ListarRelatorioAdminComponent implements OnInit {

  relatorios: RelatorioResponse[] = [];

  form!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private relatorioService: RelatorioService,
    private loadingService: LoadingService,
    private toastService: ToastService
  ) {}

  ngOnInit(): void {
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

  private toDateInputValue(date: Date): string {
    const y = date.getFullYear();
    const m = String(date.getMonth() + 1).padStart(2, '0');
    const d = String(date.getDate()).padStart(2, '0');
    return `${y}-${m}-${d}`;
  }

  private fromDateInputValue(value: string): Date {
    const [y, m, d] = value.split('-').map(Number);
    return new Date(y, m - 1, d);
  }

  private dateRangeValidator = (group: AbstractControl): ValidationErrors | null => {
    const start = group.get('start')?.value as string;
    const end = group.get('end')?.value as string;

    if (!start || !end) return null;

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


  async SolicitarRelatorio() {
    this.form.markAllAsTouched();

    if (this.form.invalid) {
      if (this.f['start']?.errors?.['required'] || this.f['end']?.errors?.['required']) {
        this.toastService.error('Preencha ambas as datas.');
      } else if (this.form.errors?.['dateRangeInvalid']) {
        this.toastService.error('A data final não pode ser menor que a inicial.')
      } else {
        this.toastService.error('Verifique os campos');
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

      this.toastService.success('Relatório solicitado com sucesso. Em breve ele estará disponível.');
    } catch (error: any) {
      this.toastService.error(ApiErrorHelper.getApiErrorMessage(error));
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
      this.toastService.error(ApiErrorHelper.getApiErrorMessage(error));
    } finally {
      this.loadingService.hide();
    }
  }
}
