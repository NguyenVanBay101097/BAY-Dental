import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LoaiThuChiManagementComponent } from './loai-thu-chi-management.component';

describe('LoaiThuChiManagementComponent', () => {
  let component: LoaiThuChiManagementComponent;
  let fixture: ComponentFixture<LoaiThuChiManagementComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LoaiThuChiManagementComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LoaiThuChiManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
