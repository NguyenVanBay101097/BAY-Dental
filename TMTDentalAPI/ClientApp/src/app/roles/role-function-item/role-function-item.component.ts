import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-role-function-item',
  templateUrl: './role-function-item.component.html',
  styleUrls: ['./role-function-item.component.css']
})
export class RoleFunctionItemComponent implements OnInit {
  @Input() item: any;
  constructor() { }

  ngOnInit() {
  }

  toggle(item: any) {
    var allCheck = this.isAllChildChecked(item);
    item.ops.forEach(op => {
      op.checked = !allCheck;
    });
  }

  isAllChildChecked(item) {
    for (var i = 0; i < item.ops.length; i++) {
      var op = item.ops[i];
      if (!op.checked) {
        return false;
      }
    }

    return true;
  }

  atLeastOneChecked(item) {
    for (var i = 0; i < item.ops.length; i++) {
      var op = item.ops[i];
      if (op.checked) {
        return true;
      }
    }

    return false;
  }
}
