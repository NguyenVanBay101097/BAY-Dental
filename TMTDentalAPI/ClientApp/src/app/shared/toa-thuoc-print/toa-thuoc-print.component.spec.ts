import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ToaThuocPrintComponent } from './toa-thuoc-print.component';

describe('ToaThuocPrintComponent', () => {
  let component: ToaThuocPrintComponent;
  let fixture: ComponentFixture<ToaThuocPrintComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ToaThuocPrintComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ToaThuocPrintComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
