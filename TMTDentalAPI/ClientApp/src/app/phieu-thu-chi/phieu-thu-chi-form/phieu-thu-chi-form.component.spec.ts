import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PhieuThuChiFormComponent } from './phieu-thu-chi-form.component';

describe('PhieuThuChiFormComponent', () => {
  let component: PhieuThuChiFormComponent;
  let fixture: ComponentFixture<PhieuThuChiFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PhieuThuChiFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PhieuThuChiFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
