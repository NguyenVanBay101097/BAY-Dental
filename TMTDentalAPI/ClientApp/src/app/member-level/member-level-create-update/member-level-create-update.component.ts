import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { forkJoin } from 'rxjs';
import { PartnerService } from 'src/app/partners/partner.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { MemberLevelService } from '../member-level.service';

@Component({
  selector: 'app-member-level-create-update',
  templateUrl: './member-level-create-update.component.html',
  styleUrls: ['./member-level-create-update.component.css']
})
export class MemberLevelCreateUpdateComponent implements OnInit {
  memberLevels: any[] = [];
  memberColors = [
    { id: '0', value: "#6C757D", checked: true },
    { id: '1', value: "#F06050", checked: false },
    { id: '2', value: "#F4A460", checked: false },
    { id: '3', value: "#F7CD1F", checked: false },
    { id: '4', value: "#6CC1ED", checked: false },
    { id: '5', value: "#814968", checked: false },
    { id: '6', value: "#EB7E7F", checked: false },
    { id: '7', value: "#2C8397", checked: false },
    { id: '8', value: "#475577", checked: false },
    { id: '9', value: "#D6145F", checked: false },
    { id: '10', value: "#30C381", checked: false },
    { id: '11', value: "#9365B8", checked: false },
  ]
  isEqualName: boolean = false;
  isEqualPoint: boolean = false;
  indexName: number;
  indexPoint: number;
  constructor(
    private memberLevelService: MemberLevelService,
    private notifyService: NotifyService,
    private modalService: NgbModal,
    private router: Router,
    private partnerService: PartnerService
  ) { }

  ngOnInit() {
    // this.memberLevels = [];
    this.loadDataFromApi();
  }
  onAddMemberLevel() {
    var level = {
      name: '',
      point: 0,
      color: '0',
    }
    this.memberLevels.push(level);
    this.isEqualName = false;
    this.isEqualPoint = false;
  }

  loadDataFromApi() {
    this.memberLevelService.get().subscribe((result) => {
      console.log(result);
      this.memberLevels = result;
      if (result.length == 0) {
        this.onAddMemberLevel();
      }
    })
  }

  onDeleteMemberLevel(index) {
    this.memberLevels.splice(index, 1);
  }

  onSave(form: NgForm) {
    if (form.invalid || this.isEqualName || this.isEqualPoint) {
      return;
    }

    var vals = this.memberLevels;
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Hạng thành viên';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn lưu thiết lập hạng thành viên?';
    modalRef.componentInstance.body2 = 'Lưu ý: Hạng thành viên của khách hàng sẽ được cập nhật ngay sau khi xác nhận';
    modalRef.result.then(() => {
      this.memberLevelService.updateMember(vals).subscribe((res) => {
        this.partnerService.checkUpdateLevel().subscribe(() => {
          this.notifyService.notify("success", "Lưu hạng thành viên thành công");
          this.memberLevels.length > 0 ?
            this.router.navigate(['member-level/list']) :
            this.router.navigate(['member-level/management']);
        });
      })
    });
  }

  onChangeName(event, i) {
    this.indexName = i;
    var count = 0;
    if (event != '') {
      this.memberLevels.forEach(item => {
        if (item.name.toLowerCase().normalize() === event.toLowerCase().normalize()) {
          count++;
        }
        count > 1 ? this.isEqualName = true : this.isEqualName = false;
      });
    }
  }

  onChangePoint(event, i) {
    this.indexPoint = i;
    var count = 0;
    if(event!=null)
    this.memberLevels.forEach(item => {
      if (item.point === event) {
        count++;
      }
      count > 1 ? this.isEqualPoint = true : this.isEqualPoint = false;
    });
  }

  // onCheckColor(item) {
  //   this.memberColors.forEach(color => {
  //     if (color.checked) {
  //       color.checked = false;
  //     }
  //   });

  //   this.memberColors.find(x => x.id === item.id).checked = !item.checked;
  // }

}
