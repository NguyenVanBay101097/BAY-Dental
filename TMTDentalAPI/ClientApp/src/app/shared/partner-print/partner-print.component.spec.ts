import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerPrintComponent } from './partner-print.component';

describe('PartnerPrintComponent', () => {
  let component: PartnerPrintComponent;
  let fixture: ComponentFixture<PartnerPrintComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerPrintComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerPrintComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
