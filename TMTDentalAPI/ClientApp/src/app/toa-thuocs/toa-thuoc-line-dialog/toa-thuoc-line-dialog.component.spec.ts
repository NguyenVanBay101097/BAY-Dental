import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ToaThuocLineDialogComponent } from './toa-thuoc-line-dialog.component';

describe('ToaThuocLineDialogComponent', () => {
  let component: ToaThuocLineDialogComponent;
  let fixture: ComponentFixture<ToaThuocLineDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ToaThuocLineDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ToaThuocLineDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
