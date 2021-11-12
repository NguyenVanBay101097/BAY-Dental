import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { UserDisplay, UserService } from 'src/app/users/user.service';

@Component({
  selector: 'app-user-profile-edit',
  templateUrl: './user-profile-edit.component.html',
  styleUrls: ['./user-profile-edit.component.css']
})
export class UserProfileEditComponent implements OnInit {

  @Input() id: string//id user
  editForm: FormGroup;
  appUserDisplay: UserDisplay;
  submitted = false;
  constructor(private fb: FormBuilder, public activeModal: NgbActiveModal, private userService: UserService) { }

  ngOnInit() {
    this.editForm = this.fb.group({
      userName: [null,Validators.required],
      name: [null,Validators.required],
      email: [null,Validators.required],
      avatar: null,
      passWord: null,
      companyId: null,
      company: [null, Validators.required],
      companies: [[]],
    })
    setTimeout(() => {
      this.getInfo();
    });

  }

  get f(){
    return this.editForm.controls;
  }

  onAvatarUploaded(data: any) {
    var fileUrl = data ? data.fileUrl : null;
    this.editForm.get('avatar').setValue(fileUrl);
  }


  submit() {
    this.submitted = true;
    var data = this.editForm.value;
    data.companyId = data.company.id;
    this.userService.update(this.id, data).subscribe(() => {
      this.activeModal.close(true);
    }, err => {
      console.log(err);
    });
  }

  getInfo() {
    if (this.id) {
      this.userService.get(this.id).subscribe(
        res => {
          this.editForm.patchValue(res);
        });
    }
  }

}
