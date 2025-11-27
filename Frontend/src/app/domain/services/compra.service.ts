import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { ApiGenericResponse, ApiResponse } from '../models/api-response.model';
import { CompraEfetuarRequest } from '../models/compra-efetuar.model';
import { CompraListarResponse } from '../models/compra-listar.model';
import { environment } from '../../../environments/environment'

@Injectable({
  providedIn: 'root'
})
export class CompraService {

  private readonly apiUrl = `${environment.apiUrl}/compra`;

  constructor(private http: HttpClient) {}

  async efetuar(compraEfetuarRequest: CompraEfetuarRequest[]): Promise<ApiResponse> {
    return await firstValueFrom(
      this.http.post<ApiResponse>(this.apiUrl, compraEfetuarRequest)
    );
  }

  async listarComprasAdmin(): Promise<ApiGenericResponse<CompraListarResponse[]>> {
    return await firstValueFrom(this.http.get<ApiGenericResponse<CompraListarResponse[]>>(this.apiUrl + "/admin"));
  }
  async listarComprasUsuario(): Promise<ApiGenericResponse<CompraListarResponse[]>> {
    return await firstValueFrom(this.http.get<ApiGenericResponse<CompraListarResponse[]>>(this.apiUrl + "/usuario"));
  }

  async cancelarCompra(compraId: number): Promise<ApiResponse> {
    return await firstValueFrom(this.http.delete<ApiResponse>(`${this.apiUrl}/${compraId}`));
  }

}
