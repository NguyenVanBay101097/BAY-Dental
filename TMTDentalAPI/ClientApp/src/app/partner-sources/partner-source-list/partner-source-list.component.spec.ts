import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerSourceListComponent } from './partner-source-list.component';

describe('PartnerSourceListComponent', () => {
  let component: PartnerSourceListComponent;
  let fixture: ComponentFixture<PartnerSourceListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerSourceListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerSourceListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
