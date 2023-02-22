import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ToaThuocCuDialogComponent } from './toa-thuoc-cu-dialog.component';

describe('ToaThuocCuDialogComponent', () => {
  let component: ToaThuocCuDialogComponent;
  let fixture: ComponentFixture<ToaThuocCuDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ToaThuocCuDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ToaThuocCuDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
