import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LoaiThuChiFormComponent } from './loai-thu-chi-form.component';

describe('AccountAccountFormComponent', () => {
  let component: LoaiThuChiFormComponent;
  let fixture: ComponentFixture<LoaiThuChiFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LoaiThuChiFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LoaiThuChiFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
