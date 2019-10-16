import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { ProductService, ProductFilter } from 'src/app/products/product.service';
import { WindowRef, WindowService, WindowCloseResult } from '@progress/kendo-angular-dialog';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import * as _ from 'lodash';
import { NotificationService } from '@progress/kendo-angular-notification';
import { ResGroupService } from '../res-group.service';
import { UserService, UserPaged } from 'src/app/users/user.service';
import { UserSimple } from 'src/app/users/user-simple';


@Component({
  selector: 'app-res-group-create-update',
  templateUrl: './res-group-create-update.component.html',
  styleUrls: ['./res-group-create-update.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class ResGroupCreateUpdateComponent implements OnInit {
  groupForm: FormGroup;
  id: string;
  listUsers: UserSimple[];

  constructor(private fb: FormBuilder, private userService: UserService,
    private groupService: ResGroupService, private windowService: WindowService,
    private route: ActivatedRoute, private notificationService: NotificationService,
    private router: Router) { }

  ngOnInit() {
    this.groupForm = this.fb.group({
      name: [null, Validators.required],
      modelAccesses: this.fb.array([]),
      users: null
    });

    this.id = this.route.snapshot.params['id'];
    if (this.id) {
      this.groupService.get(this.id).subscribe(result => {
        console.log(result);
        this.groupForm.patchValue(result);
        result.modelAccesses.forEach(line => {
          this.modelAccesses.push(this.fb.group(line));
        });
      });
    } else {
      this.groupService.defaultGet().subscribe(result => {
        console.log(result);
        this.groupForm.patchValue(result);
        result.modelAccesses.forEach(line => {
          this.modelAccesses.push(this.fb.group(line));
        });
      });
    }

    this.loadListUsers();
  }

  loadListUsers() {
    var val = new UserPaged();
    val.limit = 1000;
    this.userService.autocompleteSimple(val).subscribe(result => {
      this.listUsers = result;
    });
  }

  checkAll(val: boolean) {
    debugger;
    this.modelAccesses.controls.forEach(line => {
      line.get('permRead').patchValue(val);
      line.get('permCreate').patchValue(val);
      line.get('permWrite').patchValue(val);
      line.get('permUnlink').patchValue(val);
    });
  }


  checkAllLine(val: boolean, index) {
    var line = this.modelAccesses.controls[index];
    line.get('permRead').patchValue(val);
    line.get('permCreate').patchValue(val);
    line.get('permWrite').patchValue(val);
    line.get('permUnlink').patchValue(val);
  }

  permChange(e, index) {
  }

  get modelAccesses() {
    return this.groupForm.get('modelAccesses') as FormArray;
  }

  onSave() {
    if (!this.groupForm.valid) {
      return;
    }

    var val = this.groupForm.value;
    this.groupService.create(val).subscribe(result => {
      this.router.navigate(['res-groups/edit/', result.id]);
    });
  }

  onNew() {
    this.router.navigate(['res-groups/create']);
  }

  onUpdate() {
    if (!this.groupForm.valid) {
      return;
    }

    var val = this.groupForm.value;
    console.log(val);
    this.groupService.update(this.id, val).subscribe(() => {
      this.notificationService.show({
        content: 'Lưu thành công',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      })
      this.loadRecord();
    });
  }

  loadRecord() {
    if (this.id) {
      this.groupService.get(this.id).subscribe(result => {
        this.groupForm.patchValue(result);
        this.modelAccesses.clear();
        result.modelAccesses.forEach(line => {
          this.modelAccesses.push(this.fb.group(line));
        });
      });
    }
  }
}

