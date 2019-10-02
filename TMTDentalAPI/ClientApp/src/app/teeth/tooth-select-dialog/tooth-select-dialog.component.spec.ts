import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ToothSelectDialogComponent } from './tooth-select-dialog.component';

describe('ToothSelectDialogComponent', () => {
  let component: ToothSelectDialogComponent;
  let fixture: ComponentFixture<ToothSelectDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ToothSelectDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ToothSelectDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
