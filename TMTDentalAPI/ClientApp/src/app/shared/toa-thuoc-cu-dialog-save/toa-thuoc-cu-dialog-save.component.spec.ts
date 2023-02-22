import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ToaThuocCuDialogSaveComponent } from './toa-thuoc-cu-dialog-save.component';

describe('ToaThuocCuDialogSaveComponent', () => {
  let component: ToaThuocCuDialogSaveComponent;
  let fixture: ComponentFixture<ToaThuocCuDialogSaveComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ToaThuocCuDialogSaveComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ToaThuocCuDialogSaveComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
