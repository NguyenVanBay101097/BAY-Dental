import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BinaryFileInputComponent } from './binary-file-input/binary-file-input.component';

@NgModule({
  declarations: [BinaryFileInputComponent],
  imports: [
    CommonModule
  ],
  exports: [BinaryFileInputComponent]
})
export class CustomComponentModule { }
