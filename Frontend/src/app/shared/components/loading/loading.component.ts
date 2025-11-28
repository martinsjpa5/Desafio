import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoadingService } from '../../../core/services/loading.service';
import { PoLoadingModule } from '@po-ui/ng-components';

@Component({
  selector: 'app-loading',
  standalone: true,
  imports: [CommonModule, PoLoadingModule ],
  templateUrl: './loading.component.html',
  styleUrls: ['./loading.component.scss']
})
export class LoadingComponent {
  constructor(public loadingService: LoadingService) {}
}
