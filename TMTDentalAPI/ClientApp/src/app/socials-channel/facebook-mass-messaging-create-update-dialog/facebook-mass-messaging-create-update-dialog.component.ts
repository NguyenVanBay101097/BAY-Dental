import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-facebook-mass-messaging-create-update-dialog',
  templateUrl: './facebook-mass-messaging-create-update-dialog.component.html',
  styleUrls: ['./facebook-mass-messaging-create-update-dialog.component.css']
})
export class FacebookMassMessagingCreateUpdateDialogComponent implements OnInit {
  formGroup: FormGroup;
  massMessagingId: string;
  showAddTag: boolean = false;
  listTags: any[] = [];
  
  constructor(private fb: FormBuilder, public activeModal: NgbActiveModal) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      tag: null
    });
  }
  changeTag() {
    
  }
  onSave() {
    if (!this.formGroup.valid) {
      return false;
    }

    var value = this.formGroup.value;
    console.log(value);
  }
}
