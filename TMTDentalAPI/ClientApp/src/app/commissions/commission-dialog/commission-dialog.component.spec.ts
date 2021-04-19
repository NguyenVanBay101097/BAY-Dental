import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CommissionDialogComponent } from './commission-dialog.component';

describe('CommissionDialogComponent', () => {
  let component: CommissionDialogComponent;
  let fixture: ComponentFixture<CommissionDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CommissionDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CommissionDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
