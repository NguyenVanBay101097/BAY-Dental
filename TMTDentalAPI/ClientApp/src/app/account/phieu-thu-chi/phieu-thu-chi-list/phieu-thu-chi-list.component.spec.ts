import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PhieuThuChiListComponent } from './phieu-thu-chi-list.component';

describe('PhieuThuChiListComponent', () => {
  let component: PhieuThuChiListComponent;
  let fixture: ComponentFixture<PhieuThuChiListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PhieuThuChiListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PhieuThuChiListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
