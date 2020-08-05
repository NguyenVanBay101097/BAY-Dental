import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LoaiThuChiListComponent } from './loai-thu-chi-list.component';

describe('AccountAccountListComponent', () => {
  let component: LoaiThuChiListComponent;
  let fixture: ComponentFixture<LoaiThuChiListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LoaiThuChiListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LoaiThuChiListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
