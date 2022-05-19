import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ClipboardService } from 'ngx-clipboard';
import { NotifyService } from 'src/app/shared/services/notify.service';

@Component({
  selector: 'app-keyword-list-dialog',
  templateUrl: './keyword-list-dialog.component.html',
  styleUrls: ['./keyword-list-dialog.component.css']
})
export class KeywordListDialogComponent implements OnInit {
  search = '';
  title = 'Danh sách từ khóa';
  searchUpdate = new Subject<string>();
  @Input() boxKeyWordSource = [];
  boxKeyWordList = [];
  type: string;
  content = `<!--{{if (o.have_adult_teeth || o.have_child_teeth)}}-->
  <div class="text-center">
      {{if (o.have_adult_teeth)}}
      <div class="teeth_container">
          <div class="ham_tren">
              <div class="teethElement flex-row-reverse">
                  <!--{{for teeth in o.adult_up_right_teeth}}-->
                  {{if (teeth.is_selected)}} <div class="teeth_nameTeeth selected">{{teeth.name}}</div> {{end}}
                  {{if (!teeth.is_selected)}} <div class="teeth_nameTeeth">{{teeth.name}}</div> {{end}}
                  <!--{{end}}-->
              </div>
              <div class="chia_doi_doc"></div>
              <div class="teethElement">
                  <!--{{for teeth in o.adult_up_left_teeth}}-->
                  {{if (teeth.is_selected)}} <div class="teeth_nameTeeth selected">{{teeth.name}}</div> {{end}}
                  {{if (!teeth.is_selected)}} <div class="teeth_nameTeeth">{{teeth.name}}</div> {{end}}
                  <!--{{end}}-->
              </div>
          </div>
          <div class="duong_chia_doi"></div>
          <div class="ham_duoi">
              <div class="teethElement flex-row-reverse">
                  <!--{{for teeth in o.adult_down_right_teeth}}-->
                  {{if (teeth.is_selected)}} <div class="teeth_nameTeeth selected">{{teeth.name}}</div>{{end}}
                  {{if (!teeth.is_selected)}} <div class="teeth_nameTeeth">{{teeth.name}}</div> {{end}}
                  <!--{{end}}-->
              </div>
              <div class="chia_doi_doc"></div>
              <div class="teethElement">
                  <!--{{for teeth in o.adult_down_left_teeth}}-->
                  {{if (teeth.is_selected)}} <div class="teeth_nameTeeth selected">{{teeth.name}}</div> {{end}}
                  {{if (!teeth.is_selected)}} <div class="teeth_nameTeeth">{{teeth.name}}</div> {{end}}
                  <!--{{end}}-->
              </div>
          </div>
      </div>
  
      {{end}}
      {{if (o.have_child_teeth)}}
      <div class="teeth_container mt-1">
          <div class="ham_tren">
              <div class="teethElement flex-row-reverse">
                  <!--{{for teeth in o.child_up_right_teeth}}-->
                  {{if (teeth.is_selected)}} <div class="teeth_nameTeeth selected">{{teeth.name}}</div> {{end}}
                  {{if (!teeth.is_selected)}} <div class="teeth_nameTeeth">{{teeth.name}}</div> {{end}}
                  <!--{{end}}-->
              </div>
              <div class="chia_doi_doc"></div>
              <div class="teethElement">
                  <!--{{for teeth in o.child_up_left_teeth}}-->
                  {{if (teeth.is_selected)}} <div class="teeth_nameTeeth selected">{{teeth.name}}</div> {{end}}
                  {{if (!teeth.is_selected)}} <div class="teeth_nameTeeth">{{teeth.name}}</div> {{end}}
                  <!--{{end}}-->
              </div>
          </div>
          <div class="duong_chia_doi"></div>
          <div class="ham_duoi">
              <div class="teethElement flex-row-reverse">
                  <!--{{for teeth in o.child_down_right_teeth}}-->
                  {{if (teeth.is_selected)}} <div class="teeth_nameTeeth selected">{{teeth.name}}</div>{{end}}
                  {{if (!teeth.is_selected)}} <div class="teeth_nameTeeth">{{teeth.name}}</div> {{end}}
                  <!--{{end}}-->
              </div>
              <div class="chia_doi_doc"></div>
              <div class="teethElement">
                  <!--{{for teeth in o.child_down_left_teeth}}-->
                  {{if (teeth.is_selected)}} <div class="teeth_nameTeeth selected">{{teeth.name}}</div> {{end}}
                  {{if (!teeth.is_selected)}} <div class="teeth_nameTeeth">{{teeth.name}}</div> {{end}}
                  <!--{{end}}-->
              </div>
          </div>
      </div>
      {{end}}
  </div>
  <!--{{end}}-->`;
  constructor(
    public activeModal: NgbActiveModal,
    private clipboardApi: ClipboardService,
    private notifyService: NotifyService
  ) { }

  ngOnInit() {
    this.boxKeyWordList = this.boxKeyWordSource;
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        var temp = JSON.parse(JSON.stringify(this.boxKeyWordSource))
        temp = temp.filter(x => {
          x.value = x.value.filter(z => z.text.toLowerCase().includes(value.toLowerCase()) || z.value.toLowerCase().includes(value.toLowerCase()))
          return x.value.length > 0;
        });
        this.boxKeyWordList = temp;
      });
  }

  onSelectKeyWord(item) {
    this.activeModal.close(item);
  }

  copyText() {
    this.clipboardApi.copyFromContent(this.content);
    this.notifyService.notify('success', 'Đã copy html');
  }

}
