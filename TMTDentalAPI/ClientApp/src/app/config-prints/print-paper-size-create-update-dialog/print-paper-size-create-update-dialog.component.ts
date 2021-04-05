import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from 'src/app/auth/auth.service';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { PrintPaperSizeService } from '../print-paper-size.service';

@Component({
  selector: 'app-print-paper-size-create-update-dialog',
  templateUrl: './print-paper-size-create-update-dialog.component.html',
  styleUrls: ['./print-paper-size-create-update-dialog.component.css']
})
export class PrintPaperSizeCreateUpdateDialogComponent implements OnInit {
  id: string;
  formGroup: FormGroup;
  opened = false;
  title: string;
  submitted = false;

  constructor(private fb: FormBuilder, 
    public activeModal: NgbActiveModal, 
    private printPaperSizeService: PrintPaperSizeService,
    private authService: AuthService,
    public notifyService: NotifyService
    ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: [null, Validators.required],
      paperFormat: "A4",
      topMargin: 0,
      bottomMargin: 0,
      leftMargin: 0,
      rightMargin: 0
    });

    if(this.id){
      this.loadDataFromApi();
    }
  }

  loadDataFromApi() {
    this.printPaperSizeService.get(this.id).subscribe(res =>{
      this.formGroup.patchValue(res);
    });
  }

  onSave() {
    this.submitted = true;

    if (this.formGroup.invalid) {
      return;
    }
   

    var val = this.formGroup.value;
    val.companyId = this.authService.userInfo.companyId;

    if (this.id) {
      this.printPaperSizeService.update(this.id, val).subscribe(res => {
        this.notifyService.notify('success', 'Lưu thành công');
        this.submitted = false;
        this.activeModal.close(res);
      })
    } else {
      this.printPaperSizeService.create(val).subscribe(res => {
        this.notifyService.notify('success', 'Lưu thành công');
        this.submitted = false;
        this.activeModal.close(res);
      })
    }
  }

  get f() { return this.formGroup.controls; }


}
