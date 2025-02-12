-- =============================================
-- USERS TABLE (Stores user details)
-- =============================================
CREATE TABLE users (
    id         INT PRIMARY KEY AUTO_INCREMENT,
    name       VARCHAR(100) NOT NULL,
    email      VARCHAR(150) UNIQUE NOT NULL,
    password   VARCHAR(255) NOT NULL, 
    role       ENUM('Admin', 'Manager', 'Sales') NOT NULL DEFAULT 'Sales',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- =============================================
-- USER PERFORMANCE TABLE (Tracks sales rep efficiency)
-- =============================================
CREATE TABLE user_performance (
    id                 INT PRIMARY KEY AUTO_INCREMENT,
    user_id            INT NOT NULL,
    leads_handled      INT DEFAULT 0,  -- Total leads assigned
    leads_converted    INT DEFAULT 0,  -- Successfully converted leads
    response_time_avg  DECIMAL(5,2) DEFAULT 0,  -- Avg response time in hours
    last_updated       TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

    -- Foreign Key
    CONSTRAINT fk_performance_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);


-- =============================================
-- LEADS TABLE (Stores lead details)
-- =============================================
CREATE TABLE leads (
    id          INT PRIMARY KEY AUTO_INCREMENT,
    name        VARCHAR(100) NOT NULL,
    email       VARCHAR(150) UNIQUE NOT NULL,
    phone       VARCHAR(20) NOT NULL,
    source      ENUM('Website', 'Referral', 'Other') NOT NULL DEFAULT 'Other',
    notes  		TEXT NOT NULL,
    assigned_to INT,  
    status      ENUM('New', 'Contacted', 'Follow-up', 'Converted', 'Lost') NOT NULL DEFAULT 'New',
    created_at  TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at  TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

    -- Foreign Key
    CONSTRAINT fk_lead_assigned_to FOREIGN KEY (assigned_to) REFERENCES users(id) ON DELETE SET NULL
);


-- =============================================
-- LEAD STATUS HISTORY TABLE (Tracks lead status changes)
-- =============================================
CREATE TABLE lead_status_history (
    id          INT PRIMARY KEY AUTO_INCREMENT,
    lead_id     INT NOT NULL,
    user_id     INT NULL,
    old_status  ENUM('New', 'Contacted', 'Follow-up', 'Converted', 'Lost') NOT NULL,
    new_status  ENUM('New', 'Contacted', 'Follow-up', 'Converted', 'Lost') NOT NULL,
    changed_at  TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    -- Foreign Keys
    CONSTRAINT fk_status_lead FOREIGN KEY (lead_id) REFERENCES leads(id) ON DELETE CASCADE,
    CONSTRAINT fk_status_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE SET NULL
);

