import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { ProdutoAdicionarRequest, ProdutoEditarRequest, ProdutoListarResponse } from '../models/produto.model';
import { ApiGenericResponse, ApiResponse } from '../models/api-response.model';
import { environment } from '../../../environments/environment'

@Injectable({
  providedIn: 'root'
})
export class ProdutoService {

  private readonly apiUrl = `${environment.apiUrl}/produto`;

  constructor(private http: HttpClient) {}

  async listar(): Promise<ProdutoListarResponse[]> {
    const response = await firstValueFrom(
      this.http.get<ApiGenericResponse<ProdutoListarResponse[]>>(this.apiUrl)
    );
    
    return response.data;
  }

  async cadastrar(request: ProdutoAdicionarRequest): Promise<ApiResponse> {
      return await firstValueFrom(
        this.http.post<ApiResponse>(this.apiUrl, request)
      );
    }

    async editar(request: ProdutoEditarRequest): Promise<ApiResponse> {
      return await firstValueFrom(
        this.http.put<ApiResponse>(this.apiUrl, request)
      );
    }

    async deletar(produtoId: number): Promise<ApiResponse> {
      return await firstValueFrom(
        this.http.delete<ApiResponse>(this.apiUrl + "/" + produtoId)
      );
    }
}
