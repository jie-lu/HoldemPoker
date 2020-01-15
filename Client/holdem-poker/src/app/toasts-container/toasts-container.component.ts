import { Component, TemplateRef } from '@angular/core';
import { ToastService } from '../services/toast-service.service';

@Component({
  selector: 'app-toasts',
  templateUrl: './toasts-container.component.html',
  styleUrls: ['./toasts-container.component.less'],
  host: {'[class.ngb-toasts]': 'true'}
})
export class ToastsContainerComponent {
  constructor(public toastService: ToastService) {}

  isTemplate(toast) { return toast.textOrTpl instanceof TemplateRef; }
}