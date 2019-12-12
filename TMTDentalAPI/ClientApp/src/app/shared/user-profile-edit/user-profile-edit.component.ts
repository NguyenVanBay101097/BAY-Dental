import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { UserService, UserDisplay } from 'src/app/users/user.service';
import { ApplicationUserDisplay } from 'src/app/appointment/appointment';

@Component({
  selector: 'app-user-profile-edit',
  templateUrl: './user-profile-edit.component.html',
  styleUrls: ['./user-profile-edit.component.css']
})
export class UserProfileEditComponent implements OnInit {

  @Input() id: string//id user
  editForm: FormGroup;
  appUserDisplay: UserDisplay;
  constructor(private fb: FormBuilder, public activeModal: NgbActiveModal, private userService: UserService) { }

  ngOnInit() {
    this.editForm = this.fb.group({
      userName: null,
      name: null,
      email: null,
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

  submit() {
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
