import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { element } from 'protractor';

@Component({
  selector: 'app-role-form-v2',
  templateUrl: './role-form-v2.component.html',
  styleUrls: ['./role-form-v2.component.css']
})
export class RoleFormV2Component implements OnInit {

  @Input() listFeature: any;
  @Input() listRoleInput: string[] = [];
  @Output() listRoleOutput = new EventEmitter<string[]>();
  permissionDict: any = {};

  constructor() { }

  ngOnInit() {
    this.listRoleInput.forEach(op => {
      this.permissionDict[op] = true;
    });
    console.log(this.permissionDict);
  }

  expand(item){
    item.expand = !item.expand;
  }

  checkChild(item,childItem){
    if(childItem.checked) {
      item.amount_checked -= 1;
    }
    else {
      item.amount_checked += 1;
    }
    childItem.checked = !childItem.checked;
    if (childItem.checked) {
      this.listRoleInput.push(childItem.permission);
    } else {
      var index = this.listRoleInput.findIndex(element => element == childItem.permission);
      this.listRoleInput.splice(index, 1);
    }
    this.listRoleOutput.emit(this.listRoleInput);
  }

  check(item){
    item.checked = !item.checked;
    if (item.checked){
      item.amount_checked = item.children.length;
      item.children.forEach(elem => {
        elem.checked = true;
        this.listRoleInput.push(elem.permission);
      });
    }
    else {
      item.amount_checked = 0;
      item.children.forEach(elem => {
        elem.checked = false;
        var index = this.listRoleInput.findIndex(x => x == elem.permission);
        this.listRoleInput.splice(index, 1);
      });
    }
    this.listRoleOutput.emit(this.listRoleInput);
  }

  getClass(item){
    if ((item.amount_checked == item.children.length && item.amount_checked != 0) || item.checked == true) {
      return 'far mr-1 fa-check-square';
    }
    if (item.amount_checked != item.children.length && item.amount_checked != 0) {
      return 'far mr-1 fa-minus-square';
    }
    if (item.amount_checked == 0 ||item.checked == false) {
      return 'far mr-1 fa-square';
    }
  }

}
