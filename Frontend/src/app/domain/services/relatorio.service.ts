import { Injectable } from "@angular/core";
import { environment } from "../../../environments/environment";
import { HttpClient } from "@angular/common/http";
import { firstValueFrom } from "rxjs";
import { ApiGenericResponse, ApiResponse } from "../models/api-response.model";
import { RelatorioResponse } from "../models/relatorio-response.model";

@Injectable({
  providedIn: 'root'
})
export class RelatorioService {

  private readonly apiUrl = `${environment.apiUrl}/relatorio`;

  constructor(private http: HttpClient) {}

  async Solicitar(dataInicial: Date, dataFinal: Date): Promise<ApiResponse> {
    return await firstValueFrom(
      this.http.post<ApiResponse>(this.apiUrl, {dataInicial: dataInicial, dataFinal: dataFinal})
    );
  }

  async Obter(): Promise<ApiGenericResponse<RelatorioResponse[]>> {
    return await firstValueFrom(
      this.http.get<ApiGenericResponse<RelatorioResponse[]>>(this.apiUrl)
    );
  }
}