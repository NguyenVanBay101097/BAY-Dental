import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AudienceFilterServiceComponent } from './audience-filter-service.component';

describe('AudienceFilterServiceComponent', () => {
  let component: AudienceFilterServiceComponent;
  let fixture: ComponentFixture<AudienceFilterServiceComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AudienceFilterServiceComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AudienceFilterServiceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
