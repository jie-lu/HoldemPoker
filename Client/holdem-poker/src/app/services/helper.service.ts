import { Injectable } from '@angular/core';
import { ToastService } from './toast-service.service';
import { HttpErrorResponse } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class HelperService {

  constructor(private toastService: ToastService) { }

  public handleError(error) {
    console.log(error);
    if(error instanceof HttpErrorResponse) {
      if(typeof error.error === 'string') {
        this.toastService.show(error.error);
      } else {
        this.toastService.show(error.message);
      }
    } else {
      this.toastService.show(error);
    }
  }
}
