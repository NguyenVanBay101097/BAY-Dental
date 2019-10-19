import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { IrRuleCuDialogComponent } from './ir-rule-cu-dialog.component';

describe('IrRuleCuDialogComponent', () => {
  let component: IrRuleCuDialogComponent;
  let fixture: ComponentFixture<IrRuleCuDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ IrRuleCuDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(IrRuleCuDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
